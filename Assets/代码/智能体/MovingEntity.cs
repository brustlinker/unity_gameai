using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{

	protected Vector3  _速度;

	//一个表转化的向量，指向实体的朝向（在本例中，交通工具的朝向向量将总是与速度一致）
	protected Vector3  _朝向单位向量;

	//垂直于朝向向量的向量
	protected Vector3  _单位法向量;

	protected float    _质量;

	//实体最大速度
	protected float    _最大速度;

	//实体产生的供以自己的最大力(火箭的推力)
	protected float    _最大力;

	//最大转向速率（弧度每秒）
	protected float    _最大转向速度;

	//是否被标记
	private   bool     _tag;


	public Vector3 速度
	{
		get
		{
			return _速度;
		}
		set
		{
			_速度 = value;
		}
	}

	public Vector3 朝向单位向量
	{
		get
		{
			return _朝向单位向量;
		}
		set
		{
			_朝向单位向量 = value;
		}
	}

	public Vector3 单位法向量
	{
		get
		{
			return _单位法向量;
		}
		set
		{
			_单位法向量 = value;
		}
	}

	public float 质量
	{
		get
		{
			return _质量;
		}
		set
		{
			_质量 = value;
		}
	}

	public float 最大速度
	{
		get
		{
			return _最大速度;
		}
		set
		{
			_最大速度 = value;
		}
	}

	public float 最大力
	{
		get
		{
			return _最大力;
		}
		set
		{
			_最大力 = value;
		}
	}

	public float 最大转向速度
	{
		get
		{
			return _最大转向速度;
		}
		set
		{
			_最大转向速度 = value;
		}
	}

	public bool Tag
	{
		get
		{
			return _tag;
		}
		set
		{
			_tag = value;
		}
	}



	public void TagIt()
	{
		_tag = true;
	}

	public void UnTag()
	{
		_tag = false;
	}


}
