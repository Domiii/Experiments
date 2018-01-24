using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class MarbleProceduralTexture : BasicProcedualTexture
{
	//xPeriod and yPeriod together define the angle of the lines
	//xPeriod and yPeriod both 0 ==> it becomes a normal clouds or turbulence pattern
	public float xPeriod = 5.0f;
	//defines repetition of marble lines in x direction
	public float yPeriod = 10.0f;
	//defines repetition of marble lines in y direction
	//turbPower = 0 ==> it becomes a normal sine pattern
	public float turbPower = 5.0f;
	//initial size of the turbulence

	protected override void Reset ()
	{
		base.Reset ();

		noise.frequency = 1/32.0f;
	}

	public override Color GenColor (float x, float y, int xi, int yi)
	{
		var xyValue = xi * xPeriod / width + yi * xPeriod / height + turbPower * noise.SampleNoise (x, y, seed, seed * 2);
		var c = Mathf.Abs (Mathf.Sin (xyValue * Mathf.PI));

		return new Color (c, c, c);
	}
}