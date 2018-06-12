using UnityEngine;

namespace EntityFun.Utils
{
	public class CameraFollow : MonoBehaviour
	{
		[SerializeField]
		private Transform _targetTransform;

		private Transform _cachedTransform;

		private void Awake() 
		{
			_cachedTransform = transform;
		}

		private void LateUpdate()
		{               
            _cachedTransform.position = _targetTransform.position;  
            _cachedTransform.rotation = Quaternion.Slerp(_cachedTransform.rotation, _targetTransform.rotation, 3 * Time.deltaTime);
        }

    }
}