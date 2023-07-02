using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniScript;

public class Demo : MonoBehaviour
{
	void Start()
	{
		MiniDecoder<float>.Setup();
		MiniDecoder<float> decoder = new();
		Context<float> context = new();
		context
			.Set("a", 1)
			.Set("b", 2);
		var value = decoder.Decode("a+b").Evalute(context);
		Debug.Log(value.Value.ToString());
	}
}
