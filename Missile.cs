using System;
using UnityEngine;

namespace Tarodev {
    
    public class Missile : MonoBehaviour {
        private PlayerAI _player;
        private EnemyAI _enemy;

        [Header("REFERENCES")] 
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Rigidbody _target;
        [SerializeField] private GameObject _explosionPrefab;

        [Header("MOVEMENT")] 
        [SerializeField] private float _speed = 15;
        [SerializeField] private float _rotateSpeed = 95;

        [Header("PREDICTION")] 
        [SerializeField] private float _maxDistancePredict = 100;
        [SerializeField] private float _minDistancePredict = 5;
        [SerializeField] private float _maxTimePrediction = 5;
        private Vector3 _standardPrediction, _deviatedPrediction;

        [Header("DEVIATION")] 
        [SerializeField] private float _deviationAmount = 50;
        [SerializeField] private float _deviationSpeed = 2;


        private void Awake()
        {
            _player = GameObject.Find("PlayerObj").GetComponent<PlayerAI>();
            _target = _player.enemy.GetComponent<Rigidbody>();
            
        }
        private void FixedUpdate() 
            {
            _rb.velocity = transform.forward * _speed;
            if (_target == null) SeekTarget();
            if (_target != null)
            {
            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, _target.transform.position));

            PredictMovement(leadTimePercentage);

            AddDeviation(leadTimePercentage);

            RotateRocket();
            }
        }
        private void SeekTarget()
        {
            if (_player.enemy != null)
            _target = _player.enemy.GetComponent<Rigidbody>();
            else Destroy(this);
        }

        private void PredictMovement(float leadTimePercentage) {
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);

            _standardPrediction = _target.position + _target.velocity * predictionTime;
        }

        private void AddDeviation(float leadTimePercentage) {
            var deviation = new Vector3(Mathf.Cos(Time.time * _deviationSpeed), 0, 0);
            
            var predictionOffset = transform.TransformDirection(deviation) * _deviationAmount * leadTimePercentage;

            _deviatedPrediction = _standardPrediction + predictionOffset;
        }

        private void RotateRocket() {
            var heading = _deviatedPrediction - transform.position;

            var rotation = Quaternion.LookRotation(heading);
            _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _rotateSpeed * Time.deltaTime));
        }

        private void OnCollisionEnter(Collision collision) 
        {
            if (collision.gameObject.CompareTag("Enemy"))
                {
            _enemy = collision.gameObject.GetComponent<EnemyAI>();
            _enemy.TakeDamage(_player.damage);
                if(_explosionPrefab) Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
                if (collision.transform.TryGetComponent<iExplode>(out var ex)) ex.Explode();
   
                Destroy(gameObject);
            }
                if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Ground"))
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>());
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _standardPrediction);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_standardPrediction, _deviatedPrediction);
        }
    }
}