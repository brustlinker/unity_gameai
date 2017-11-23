using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

	//evade
	public bool 打开evade;
	public float evade权重;
	public Vehicle evade目标;

	//wander
	//wander圈的半径
	public float wander半径 = 3 ;
	//距离智能体的距离
	public float wander距离;
	//美妙加到目标的随机位移的最大值
	public float wanderJitter;

	public bool 打开wander;
	public float wander权重;
	public Vehicle wander目标;

	private Vector3 wanderForce;
	public bool 打开绘制wander辅助线 = true;


	//obstacle avoidance
	public float 最小检测盒长度 = 1f;
    private float 最大float   = 100000f;


	public bool 打开obstacleAvoid;
	public float obstacleAvoid权重;

	public GameObject 交点预设;
	private GameObject 交点实例;




	public float 恐惧距离=2f;




	void Start()
    {
		vehicle = this.GetComponent<Vehicle>();
		InvokeRepeating("更新wanderForce" , 0 , 0.5f );

		交点实例 = Instantiate( 交点预设  , transform.position,Quaternion.identity);
		交点实例.transform.SetParent( this.transform );
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
		if ( 打开evade )
		{
			force += Evade( evade目标) * evade权重;
		}
		if ( 打开wander )
		{
			force += Wander( ) * wander权重;
		}
		if ( 打开obstacleAvoid )
		{
			force += ObstacleAvoidance( ) * obstacleAvoid权重;
		}

		return force;
	}


/*************************************************************************************************


seek


*************************************************************************************************/



	Vector3 Seek( Vector3 目标位置 )
	{
		Vector3 预期速度 = Vector3.Normalize( 目标位置 - transform.position ) * vehicle.最大速度;
		return 预期速度 - vehicle.速度;
	}



/*************************************************************************************************


flee


*************************************************************************************************/


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



/*************************************************************************************************


arrive


*************************************************************************************************/




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





/*************************************************************************************************


pursuit


*************************************************************************************************/





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



/*************************************************************************************************


evade


*************************************************************************************************/





	Vector3 Evade( Vehicle 追逐者 )
	{
		Vector3 相差向量 = 追逐者.transform.position - transform.position;

		//预测逃避着的位置
		//预测的时间正比于逃避着和追逐着的距离；反比于智能体的速度和
		float 预测时间 = 相差向量.magnitude / ( vehicle.速度.magnitude + 追逐者.速度.magnitude );

		return Flee ( 追逐者.transform.position + 追逐者.速度 * 预测时间 );
	}



/*************************************************************************************************


wander


*************************************************************************************************/
	Vector3	Wander()
	{

		return wanderForce;
	}



	void 更新wanderForce()
	{
		//A:首先，加一个小的随机向量到目标位置
		//wander是一个点，被限制半径为 wander半径 的圈上，
		Vector3 wanderTarget = Vector3.zero;
	    wanderTarget += new Vector3( Random.Range(-1f , 1f ) * wanderJitter
			,Random.Range( - 1f , 1f ) * wanderJitter ,  0 );

		//把这个新的向量重新投影刀单元圆周上
		wanderTarget = wanderTarget.normalized * wander半径;

		//移动目标到智能体前面 wander距离 的位置
		Vector3 单位速度 = vehicle.速度.normalized;
		Vector3 targetLocal = new Vector3 (wander距离 * 单位速度.x, wander距离 * 单位速度.y, 0 );

		//把目标投影到世界空间
		Vector3 targetWorld = transform.position + targetLocal + wanderTarget;


		//移动向他
		wanderForce =  targetWorld - transform.position;


	}



/*************************************************************************************************


ObstacleAvoidance


*************************************************************************************************/



	private 障碍物 需绘制最近障碍物;

	Vector3 ObstacleAvoidance()
	{


		float 检测盒长度 = 最小检测盒长度 + (vehicle.速度.magnitude / vehicle.最大速度 ) * 最小检测盒长度;

		//标记在范围内的所有障碍物
		List<障碍物> 标记障碍物_list = 标记障碍物(  检测盒长度 );

		//跟踪最近的想叫障碍物
		障碍物 相交最近障碍物 = null;

		//跟踪到 相交最近的障碍物 的距离
		double 到相交最近的障碍物的距离 = 最大float;


		//记录 相交最近障碍物 被转化为局部坐标
		Vector3 相交最近障碍物_局部坐标  = new Vector3( 最大float , 最大float , 0 ) ;
		foreach(障碍物 障碍物 in 标记障碍物_list)
		{
			//计算这个障碍物在局部发空间的位置
			Vector3 LocalPos = PointToLocalSpace( 障碍物.中心点 , vehicle.速度 , transform.position );

			//局部坐标小雨0
			if(LocalPos.x < 0)
			{
				continue;
			}


			float 障碍物扩展半径 =  障碍物.半径 + vehicle.宽度();

			//不相交
			if( Mathf.Abs( LocalPos.y ) >= 障碍物扩展半径 )
			{
				continue;
			}

			//圆周的中心是（cx，cy）
			//交点的公式是 x = cx +/- sqrt( r * r - cy * cy )
			//我们要看的是最近的值，所以，我们试试那个是最近的值
			float cx = LocalPos.x;
			float cy = LocalPos.y;


			//我们只需要一次计算上面等式的开发
			float 开方部分 = Mathf.Sqrt( 障碍物扩展半径*障碍物扩展半径 - cy*cy );

			float 交点 = cx - 开方部分 ;
			if( 交点 <= 0)
			{
				交点 = cx + 开方部分;
			}

			//比较是否为目前为止的最近
			//如果是，记录这个障碍物和它的局部坐标
			if( 交点 < 到相交最近的障碍物的距离 )
			{
				到相交最近的障碍物的距离 = 交点;
				相交最近障碍物 = 障碍物;
				相交最近障碍物_局部坐标 = LocalPos;
			}

		}//foreach 障碍物


		//计算操控力
		Vector3 操控力= Vector3.zero;


		需绘制最近障碍物 = 相交最近障碍物;

		if( 相交最近障碍物 != null )
		{
			交点实例.transform.position = 相交最近障碍物_局部坐标;
			
			//智能体离物体越近，操控里就越强
			float multiplier = 1.0f + ( 检测盒长度 - 相交最近障碍物_局部坐标.x ) / 检测盒长度;

			//侧向力
			操控力.y = ( 相交最近障碍物.半径 - 相交最近障碍物_局部坐标.y ) * multiplier;


			//制动力,正比喻障碍物到交通工具的距离
			const float BrakingWeight = 0.2f;

			操控力.x = ( 检测盒长度 - 相交最近障碍物_局部坐标.x ) * BrakingWeight;

		}//if 相交最近障碍物

		//把操控向量从局部空间转化到世界空间


		return VectorToWorldSpace( 操控力 , vehicle.速度 , transform.position );

	}

	List<障碍物> 标记障碍物( float 检测盒子长度 )
	{
		List<障碍物> 标记障碍物_list = new List<障碍物>();

		foreach(障碍物 障碍物 in 环境.实例.障碍物_list)
		{
			if ( Vector3.Distance( 障碍物.中心点 , transform.position) < 检测盒子长度 )
			{
				标记障碍物_list.Add( 障碍物 );
			}
		}

		return 标记障碍物_list;
	}



	/// <summary>
	/// 这个函数有问题
	/// </summary>
	/// <returns>The to local space.</returns>
	/// <param name="中心点">中心点.</param>
	/// <param name="向量b">向量b.</param>
	/// <param name="向量b起点">向量b起点.</param>
	Vector3 PointToLocalSpace( Vector3 中心点 ,Vector3 向量b , Vector3 向量b起点 )
	{
		//向量在另一个向量上的投影

		Vector3 向量a = 中心点 - 向量b起点;

		float A = Mathf.Atan2( 向量a.y , 向量a.x );
		float B = Mathf.Atan2( 向量b.y , 向量b.x );

		float 夹角 = A - B;

		float x = 向量a.magnitude * Mathf.Cos( 夹角 );
		float y = 向量a.magnitude * Mathf.Sin( 夹角 );

	 	return new Vector3( x , y , 0 );
	}


	/// <summary>
	/// 没有考虑除0错误
	/// </summary>
	/// <returns>The to world space.</returns>
	/// <param name="相对向量">相对向量.</param>
	/// <param name="坐标向量">坐标向量.</param>
	/// <param name="坐标向量起点">坐标向量起点.</param>
	Vector3  VectorToWorldSpace( Vector3 相对向量 ,Vector3 坐标向量, Vector3 坐标向量起点 )
	{

		//计算两个角的和
		//计算相对向量的角
		//
		float 相对向量的角 = Mathf.Atan2(相对向量.y,相对向量.x);
		//计算坐标向量的角
		float 坐标向量的角 = Mathf.Atan2(坐标向量.y,坐标向量.x);
		//计算两个角的和
		float 和 = 相对向量的角 + 坐标向量的角;

		float x = 相对向量.magnitude* Mathf.Cos( 和 );
		float y = 相对向量.magnitude* Mathf.Sin( 和 );

		return new Vector3(x,y,0);
	}




	void 绘制ObstaclAvoid辅助线()
	{
		//关闭烦人的error警告
		if( vehicle ==null )
			return;


		//测试转化世界坐标函数
		//转化世界坐标没有问题
		Vector3 偏移量 = VectorToWorldSpace( new Vector3( -1 , -2 , 0 ) , vehicle.速度 , transform.position );
		Gizmos.DrawCube ( transform.position  + 偏移量 , new Vector3(0.3f,0.3f,0.3f)  );


		//测试交点，
		//当点击按钮的时候显示一个图片为交点

		//实例化一个prefab，
		//获取prefab的相对位置
		//计算出prefab的绝对位置
		//设置交点图片的位置


		//在测试转化为局部坐标(也是正确的)
		//Debug.Log( PointToLocalSpace( transform.position + 偏移量,vehicle.速度,vehicle.transform.position));

		//绘制检测盒子
		float 检测盒长度 = 最小检测盒长度 + (vehicle.速度.magnitude / vehicle.最大速度 ) * 最小检测盒长度;
		float 速度夹角  = Mathf.Atan2( vehicle.速度.y , vehicle.速度.x  );
		Gizmos.DrawLine(transform.position, transform.position + new Vector3( 检测盒长度 * Mathf.Cos( 速度夹角 )
			, 检测盒长度 * Mathf.Sin( 速度夹角 ) , 0 ));


		//绘制最近障碍物

		if(需绘制最近障碍物!=null)
		{

		}

		//绘制obstacle avoid 力
		Vector3 obstacleAvoid = ObstacleAvoidance();
		//偏向力，制动力
		//Gizmos.DrawLine(transform.position, transform.position + new Vector3( obstacleAvoid.x , 0 , 0 ));
		Gizmos.DrawLine(transform.position, transform.position + new Vector3( 0 , obstacleAvoid.y , 0 ));

	}




/*************************************************************************************************


Gizmos 层


*************************************************************************************************/



	void OnDrawGizmos()
	{
		绘制wander辅助线();
		绘制ObstaclAvoid辅助线();
	}


	void 绘制wander辅助线()
	{
		if(!打开绘制wander辅助线)
		{
			return;
		}

		//关闭烦人的警告
		if( vehicle == null )
		{
			return;
		}

		// 设置颜色
		Gizmos.color = Color.green;


		//计算绘制圆圈的中心偏移量
		//移动距离
		Vector3 单位速度 = vehicle.速度.normalized;
		Vector3 offset= new Vector3 (wander距离 * 单位速度.x, wander距离 * 单位速度.y, 0 );

		//Vector3 offset = new Vector3(wanderParameter.Distance * forward.y,
		//    wanderParameter.Distance * forward.x,0);



		// 绘制圆环
		Vector3 beginPoint =   transform.position;
		Vector3 firstPoint =   transform.position;

		//转一圈
		float m_Theta = 0.1f;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
		{
			//计算
			float x = wander半径 * Mathf.Cos(theta);
			float y = wander半径 * Mathf.Sin(theta);

			Vector3 endPoint = transform.position +offset + new Vector3(x , y, 0);
			if (theta == 0)
			{
				firstPoint = endPoint;
			}
			else
			{
				Gizmos.DrawLine(beginPoint, endPoint);
			}
			beginPoint = endPoint;
		}
		// 绘制最后一条线段
		Gizmos.DrawLine(firstPoint, beginPoint);

		//再画wander直线
		Vector3 wanderForce=Wander();
		Gizmos.DrawCube ( transform.position  + wanderForce , new Vector3(0.3f,0.3f,0.3f)  );
		Gizmos.DrawLine(transform.position, transform.position + new Vector3(wanderForce.x,wanderForce.y,0));



	}





}
