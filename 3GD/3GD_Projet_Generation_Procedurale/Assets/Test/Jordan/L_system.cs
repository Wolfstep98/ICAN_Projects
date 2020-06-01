using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_system : MonoBehaviour {

	private string axiom = "F";
	private float angle;
	private string currentString;
	private Dictionary<char,string> rules = new Dictionary<char,string>();
	private Stack<TransformInfo> transformStack = new Stack<TransformInfo>();

	private float length;

	public GameObject branche;

	// Use this for initialization
	void Start () {
		
		rules.Add('F',"FF+[+F-F-F]-[-F+F+F]");
		currentString = axiom;

		angle = 25f;
		length = 10f;

		Generate();
		Generate();
		Generate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Generate(){

		length = length / 2;

		string newString = "";

		char[] stringCharacters = currentString.ToCharArray();

		for (int i = 0; i < stringCharacters.Length; i++)
		{
			char currentCharacter = stringCharacters[i];

			if(rules.ContainsKey (currentCharacter)){
				newString += rules[currentCharacter];
				} else {
					newString += currentCharacter.ToString();
				}
		}

		currentString = newString;
		Debug.Log(currentString);

		stringCharacters = currentString.ToCharArray();

		for(int i = 0; i < stringCharacters.Length; i++){

			char currentCharacter = stringCharacters [i];

			if (currentCharacter == 'F'){
				Vector3 initialPosition = transform.position;
				transform.Translate (Vector3.forward * length);
				Debug.DrawLine(initialPosition, transform.position, Color.green, 10000f,false);

			} else if (currentCharacter == '+' ){

				transform.Rotate(Vector3.up * angle);

			} else if (currentCharacter == '-'){

				transform.Rotate(Vector3.up * -angle);

			} else if (currentCharacter == '['){

				TransformInfo ti = new TransformInfo();
				ti.position = transform.position;
				ti.rotation = transform.rotation;

				transformStack.Push (ti);
			} else if (currentCharacter == ']'){

				TransformInfo ti = transformStack.Pop();

				transform.position = ti.position;
				transform.rotation = ti.rotation;
			}

		}

	}
}
