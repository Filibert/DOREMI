using UnityEngine;

public class NormDistrib
{
	private float _mean;
	private float _std;

	public NormDistrib(float mean = 0, float std = 0)
	{
		_mean = mean;
		_std = std;
	}
	
	public float Value()
	{
		float u, v, S;

		do
		{
			u = 2.0f * Random.value - 1.0f;
			v = 2.0f * Random.value - 1.0f;
			S = u * u + v * v;
		}
		while (S >= 1.0f);

		float fac = Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);
		float result = (u * fac * _std) + _mean;
		
		Mathf.Clamp(result, _mean - 3 * _std, _mean + 3 * _std);
		
		return result;
	}
}
