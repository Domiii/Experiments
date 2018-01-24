using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WoodProceduralTexture : BasicProcedualTexture
{
	public float xyPeriod = 12.0f; //number of rings
	public float turbPower = 0.1f; //makes twists

	protected override void Reset ()
	{
		base.Reset ();

		noise.frequency = 1/32.0f;
	}

	public override Color GenColor (float x, float y, int xi, int yi)
	{
		float xValue = (xi - width / 2.0f) / (float)width;
		float yValue = (yi - height / 2.0f) / (float)height;
		float distValue = Mathf.Sqrt(xValue * xValue + yValue * yValue) + turbPower * noise.SampleNoise(x, y, seed, seed*2);
		float sineValue = .5f * Mathf.Abs(Mathf.Sin(2 * xyPeriod * distValue * Mathf.PI));

		var r = (80)/256.0f + sineValue;
		var g = (30)/256.0f + sineValue;
		var b = 30/256.0f;

		return new Color (r,g,b);
	}
}