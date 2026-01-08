using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkUI : MonoBehaviour
{
    [SerializeField] Graphic ui;
	[SerializeField] float offInterval = 0.5f;
	[SerializeField] float onInterval = 0.75f;

	void Start()
	{
		ui = GetComponent<Text>();
		StartCoroutine(BlinkText());
	}

	public IEnumerator BlinkText()
	{
		while (true)
		{			
			ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, 0);
			yield return new WaitForSecondsRealtime(offInterval);
			ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, 1);
			yield return new WaitForSecondsRealtime(onInterval);
		}
	}
}
