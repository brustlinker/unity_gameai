using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


#if UNITY_EDITOR

using Sirenix.Utilities.Editor;


#endif

public class 障碍物
{
	public Vector3 中心点;
	public float   半径;
}


public class 环境 : MonoBehaviour {

	[ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddButton")]
	public List<障碍物> 障碍物_list;


	void Start ()
	{
		障碍物_list = new List<障碍物>();


		障碍物 障碍物1 = new 障碍物();
		障碍物1.中心点 = new Vector3( 2 ,  1 , 1 );
		障碍物1.半径   = 1;
		障碍物_list.Add( 障碍物1 );

		障碍物 障碍物2 = new 障碍物();
		障碍物2.中心点 = new Vector3( 3 , -1 , 1 );
		障碍物2.半径   = 0.5f;
		障碍物_list.Add( 障碍物2 );

		障碍物 障碍物3 = new 障碍物();
		障碍物3.中心点 = new Vector3(-3 , -1, 1 );
		障碍物3.半径   = 1.5f ;
		障碍物_list.Add( 障碍物3 );

		障碍物 障碍物4 = new 障碍物();
		障碍物4.中心点 = new Vector3( -3 , 3 , 1 );
		障碍物4.半径   = 0.5f ;
		障碍物_list.Add( 障碍物4 );
	}
		


	void OnDrawGizmos()
	{
		绘制圆形( new Vector3( 2 ,  1 , 1 ),   1 );
		绘制圆形( new Vector3( 3 , -1 , 1 ),0.5f );
		绘制圆形( new Vector3( -3 , -1, 1 ),1.5f );
		绘制圆形( new Vector3( -3 , 3 , 1 ),0.5f );
	}



	void 绘制圆形(Vector3 中心点 , float 半径)
	{

		// 设置颜色
		Gizmos.color = Color.green;

		// 绘制圆环
		Vector3 beginPoint =   transform.position;
		Vector3 firstPoint =   transform.position;

		//转一圈
		float m_Theta = 0.1f;
		for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
		{
			//计算
			float x = 半径 * Mathf.Cos(theta);
			float y = 半径 * Mathf.Sin(theta);

			Vector3 endPoint = 中心点 + new Vector3(x , y, 0);
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
	}

}
