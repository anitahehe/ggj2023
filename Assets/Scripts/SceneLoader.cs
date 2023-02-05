using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[SerializeField] string sceneName = "MainScene";
	[SerializeField] float fadeDuration = 1;
	[SerializeField] CanvasGroup fader;

	bool pressed;
	public void LoadScene()
	{
		if (pressed) return;

		pressed = true;
		StartCoroutine(DoLoadScene());
	}

	IEnumerator DoLoadScene()
	{
		float timer = 0;
		fader.alpha = 0;

		while (timer < fadeDuration)
		{
			fader.alpha = (timer / fadeDuration);

			timer += Time.deltaTime;
			yield return null;
		}

		fader.alpha = 1;

		SceneManager.LoadScene(sceneName);
	}
}
