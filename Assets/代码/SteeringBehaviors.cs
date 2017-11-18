using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
	//内部属性
	private Vehicle vehicle;


	//可配置属性


	//属性配置
	//seek
	public bool 打开seek;
	public float seek权重;
	public Transform seek目标;


	//pursuit
	public bool 打开pursuit;
	public float pursuit权重;
	public Vehicle pursuit目标;



	public float 恐惧距离=2f;




	void Start()
    {
		vehicle = this.GetComponent<Vehicle>();
    }

	public Vector3 计算合力()
	{
		Vector3 force = Vector3.zero;

		if (打开seek)
		{
			force += Seek( seek目标.position) * seek权重;
		}
		if ( 打开pursuit ) 
		{
			force += Pursuit( pursuit目标) * pursuit权重;
		}
			
		return force;
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




	Vector3 Pursuit( Vehicle 逃避者 )
	{

		//逃避者在前面，而且面对着智能体
		//我们可以正好靠近逃避着的当前位置
		Vector3 相差向量 = 逃避者.transform.position - transform.position;

		float cos_angle =  Vector3.Dot(  vehicle.速度.normalized , 逃避者.速度.normalized );

		//相差距离在-18到0度之间
		if( Vector3.Dot( 相差向量 , vehicle.速度 ) > 0
			&& cos_angle <-0.95
		)
		{
			//追逐当前位置
			return Seek (  逃避者.transform.position );
		}


		//预测逃避着的位置
		//预测的时间正比于逃避着和追逐着的距离；反比于智能体的速度和
		float 预测时间 = 相差向量.magnitude / ( vehicle.速度.magnitude + 逃避者.速度.magnitude );

		return Seek ( 逃避者.transform.position + 逃避者.速度 * 预测时间 );
	}





















}
