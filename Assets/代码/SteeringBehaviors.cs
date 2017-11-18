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

		return  Arrive( 目标.position , 减速类型.普通 );
	}




	Vector3 Seek( Vector3 目标位置 )
	{
		Vector3 预期速度 = Vector3.Normalize( 目标位置 - transform.position ) * vehicle.最大速度;
		return 预期速度 - vehicle.速度;
	}


	Vector3 Flee(Vector3 目标位置)
	{
		Vector3 预期速度 = Vector3.Normalize(  transform.position - 目标位置  ) * vehicle.最大速度;
		return 预期速度 - vehicle.速度;
	}

	public float 恐惧距离=2f;

	Vector3 FearFlee(Vector3 目标位置)
	{
		//float  恐惧距离的平方 = 恐惧距离 * 恐惧距离;

		Debug.Log ( Vector3.Distance( transform.position , 目标位置 ) );

		if( Vector3.Distance( transform.position , 目标位置 ) > 恐惧距离 )
		{
			return Vector3.zero;
		}


		Vector3 预期速度 = Vector3.Normalize(  transform.position - 目标位置  ) * vehicle.最大速度;
		return 预期速度 - vehicle.速度;
	}

	enum 减速类型{ 慢 = 3 , 普通 = 2 , 快速 = 1 };


	/// <summary>
	/// Arrive the specified 目标位置 and 减速类型.
	/// 距离越近速度越小。
	/// 当距离非常远的时候，不超过最大速度
	/// </summary>
	/// <param name="目标位置">目标位置.</param>
	/// <param name="减速类型">减速类型.</param>
	Vector3 Arrive( Vector3 目标位置 , 减速类型 减速类型 )
	{
		Vector3 相差向量 = 目标位置 - transform.position;

		float 距离 = 相差向量.magnitude;

		if (距离 > 0) {
			const float 减速常数 = 0.3f;

			float speed = 距离 / (float)减速类型 * 减速常数;

			//确保速度不超过最大值
			speed = Mathf.Min (vehicle.最大速度 , speed);

			Vector3 预期速度 = 相差向量 * speed / 距离;
			return 预期速度 - vehicle.速度;

		} 
		else 
		{
			return Vector3.zero;
		}

	}





















}
