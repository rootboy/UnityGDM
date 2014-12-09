using UnityEngine;

/// <summary>
/// Extension for UnityEngine's build-in class.
/// </summary>
public static class Extension
{
	public static void AddPositionX(this Transform t, float x)
	{
		t.position += new Vector3(x,0,0);
	}

	public static void AddPositionY(this Transform t, float y)
	{
		t.position += new Vector3(0,y,0);
	}

	public static void AddPositionZ(this Transform t, float z)
	{
		t.position += new Vector3(0,0,z);
	}

	public static void AddLocalPositionX(this Transform t, float x)
	{
		t.localPosition += new Vector3(x,0,0);
	}

	public static void AddLocalPositionY(this Transform t, float y)
	{
		t.localPosition += new Vector3(0,y,0);
	}

	public static void AddLocalPositionZ(this Transform t, float z)
	{
		t.localPosition += new Vector3(0,0,z);
	}
}