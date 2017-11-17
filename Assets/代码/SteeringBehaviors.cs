using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour 
{
	private Vehicle vehicle;

	void Start()
	{
		vehicle = this.GetComponent<Vehicle>();
	}



	public Transform 目标;




	public Vector3 计算合力()
	{
		
		return  Seek( 目标.position );
	}




	Vector3 Seek( Vector3 目标位置 )
	{
		Vector3 预期速度 = Vector3.Normalize( 目标位置 - transform.position ) * vehicle.最大速度;
		return 预期速度 - vehicle.速度;
	}
}
