﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MovingEntity {

	//private 世界 世界;

	SteeringBehaviors steering;

	void Start()
	{
		速度    = Vector3.zero;
		质量    = 1;
		最大速度 = 3;
		最大力   = 5;
		steering = this.GetComponent<SteeringBehaviors>();
	}



	void FixedUpdate()
	{
		Vector3 合力  = steering.计算合力();

		合力  = 交警调力 ( 合力 );
		
		Vector3 加速度 = 合力 / _质量;

		速度 += 加速度 * Time.deltaTime;

		速度 = 交警调速( 速度 );

		//确保速度不超过
		速度 = 交警调速( 速度 );

		this.transform.position = this.transform.position + _速度 * Time.deltaTime;


		更新朝向();
	}

	private Vector3 交警调力(Vector3 力)
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(力.sqrMagnitude > 2 * 最大力*最大力)
		{
			return 力.normalized * 最大力;
		}
		else
		{
			return 力;
		}
	}

	Vector3 交警调速(Vector3 速度 )
	{
		//利用速度的大小的平方来比较，省去了开平方根的消耗时间
		if(速度.sqrMagnitude > 2 * 最大速度*最大速度)
		{
			return 速度.normalized * 最大速度;
		}
		else
		{
			return 速度;
		}
	}

	private void 更新朝向()
	{
		//计算出夹角(如果利用Atan只能处理1、4象限,而且不用处理除0错误)
		float radians = Mathf.Atan2( _速度.y , _速度.x );
		//转化为角度
		float degrees = radians * Mathf.Rad2Deg;

		//旋转
		this.transform.rotation=Quaternion.Euler(0,0,degrees);
	}



}
