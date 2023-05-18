using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {
	#region PROPERTIES
	public static GameManager Instance { get; private set; }

	//create references for drag object, UI, Pop up panels

	[Header("Drag Objects")]
	[SerializeField] internal GameObject DragBucket;
	[SerializeField] internal GameObject[] DragObjects;
	[SerializeField] ImageAndNameData[] ImageAndNameData;

	[Header("UI")]
	[SerializeField] internal TextMeshProUGUI TargetName;
	[SerializeField] internal AudioSource TargetAudioSource;
	[SerializeField] internal TextMeshProUGUI TimerTxt;
	[SerializeField] internal TextMeshProUGUI QuestionCount;

	[Header("Correct Wrong Pop Up Panel")]
	[SerializeField] internal GameObject CWPopUpPanel;
	[SerializeField] internal TextMeshProUGUI CWPopUpText;

	[Header("Congrats Pop Up Panel")]
	[SerializeField] internal GameObject CongratsPopUpPanel;
	[SerializeField] internal TextMeshProUGUI CongratsPopUpText;

	List<int> selectedIndexs;
	List<int> askedQuesIndexsInThisLevel;
	float timeLimit = 16; //set the timer
	float timeLeft;
	float spentTime;

	bool isTimerStarted;
	CURRENT_LEVEL currentLevel;
	int askedQustionCount;
	int correctAnswersCount;
	int totalPoints;
	int pointsForThisQuestion;

	[Header("Progress Map")]
	[SerializeField] private GameObject ProgressmapPanel;
	[SerializeField] private GameObject[] ProgressImgArray;
	[SerializeField] internal TextMeshProUGUI ProgressDiscription;
	int currentLvelProgress;


	enum CURRENT_LEVEL {
		SoundSet, Letters_3_Words, Letters_4_Words, Letters_5_Words, Compound_Words
	}

	#endregion


	#region UNITY METHODS
	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}

		selectedIndexs = new List<int>();
		askedQuesIndexsInThisLevel = new List<int>();

		currentLvelProgress = PlayerPrefs.GetInt("currentLvelProgress", 0);
	}


	private void Start() {
		Reset();
	}


//runs this in every frame
	private void Update() {
		if (isTimerStarted) {
			timeLeft -= Time.deltaTime;
		}
		TimerTxt.text = "Time: " + ((int)timeLeft).ToString();

		if (timeLeft < 0) {
			ReloadQuiz();
			spentTime += timeLimit;
		}
	}


	public void Reset() {
		currentLevel = CURRENT_LEVEL.SoundSet;
		askedQustionCount = 0;
		correctAnswersCount = 0;
		totalPoints = 0;
		askedQuesIndexsInThisLevel.Clear();
		spentTime = 0;
		ReloadQuiz();
	}

	//retry the quiz
	public void ReTry() {
		currentLevel = CURRENT_LEVEL.Letters_3_Words;
		askedQustionCount = 3;
		correctAnswersCount = 3;
		totalPoints = 3;
		askedQuesIndexsInThisLevel.Clear();
		spentTime = 0;
		ReloadQuiz();
	}

	public void ReloadQuiz() {

		//check if the 15 questions are given
		if (askedQustionCount == 15) {
			SetCongratsPopUpPanel();
			return;
		}

		//clearing list to start with fresh random questions
		selectedIndexs.Clear();

		//select a random object
		int correctObject = UnityEngine.Random.Range(0, DragObjects.Length);

		for (int i = 0; i < DragObjects.Length; i++) {

			int ranDataIndex;

			//a random index selected to ensure they are not previously selected in the current level
			do {
				ranDataIndex = UnityEngine.Random.Range(0, ImageAndNameData[(int)currentLevel].mImageAndNameDataSet.Length);
			} while (selectedIndexs.Contains(ranDataIndex) || askedQuesIndexsInThisLevel.Contains(ranDataIndex));

			selectedIndexs.Add(ranDataIndex);


			DragObjects[i].GetComponent<Image>().sprite = ImageAndNameData[(int)currentLevel].mImageAndNameDataSet[ranDataIndex].mImage;

			DragObjects[i].transform.localPosition = DragObjects[i].GetComponent<DragObject>().defaultPosition;

			if (i == correctObject) {
				DragObjects[i].GetComponent<DragObject>().isCorrectObject = true; //flag of the DragObject component is set to true
				TargetName.text = ImageAndNameData[(int)currentLevel].mImageAndNameDataSet[ranDataIndex].mName; //set to the name from the ImageAndNameData
				TargetAudioSource.clip = ImageAndNameData[(int)currentLevel].mImageAndNameDataSet[ranDataIndex].mAudioClip; //clip is set to the audio clip from the ImageAndNameData.
				askedQuesIndexsInThisLevel.Add(ranDataIndex); ; //added to the asked q list
				TargetAudioSource.Play(); //play the audio clip from the selected one
			} else {
				DragObjects[i].GetComponent<DragObject>().isCorrectObject = false;
			}
		}


		timeLeft = timeLimit;
		isTimerStarted = true;
		TimerTxt.gameObject.SetActive(true);


		if (currentLevel == CURRENT_LEVEL.SoundSet) {
			pointsForThisQuestion = 1;

		} else if (currentLevel == CURRENT_LEVEL.Letters_3_Words) {
			pointsForThisQuestion = 2;

		} else if (currentLevel == CURRENT_LEVEL.Letters_4_Words) {
			pointsForThisQuestion = 3;

		} else if (currentLevel == CURRENT_LEVEL.Letters_5_Words) {
			pointsForThisQuestion = 4;

		} else if (currentLevel == CURRENT_LEVEL.Compound_Words) {
			pointsForThisQuestion = 5;
		}

		askedQustionCount++;


		if (askedQustionCount > 11) {
			currentLevel = CURRENT_LEVEL.Compound_Words;
			askedQuesIndexsInThisLevel.Clear();
		} else if (askedQustionCount > 8) {
			currentLevel = CURRENT_LEVEL.Letters_5_Words;
			askedQuesIndexsInThisLevel.Clear();
		} else if (askedQustionCount > 5) {
			currentLevel = CURRENT_LEVEL.Letters_4_Words;
			askedQuesIndexsInThisLevel.Clear();
		} else if (askedQustionCount > 2) {
			currentLevel = CURRENT_LEVEL.Letters_3_Words;
			askedQuesIndexsInThisLevel.Clear();
		}

		QuestionCount.text = "Question: " + askedQustionCount;
	}

	internal void IsCorrectDrag(bool state) {
		if (state) {
			CWPopUpText.text = "Correct Answer !";
			CWPopUpPanel.transform.GetChild(2).gameObject.SetActive(true);
			CWPopUpPanel.transform.GetChild(3).gameObject.SetActive(false);
			correctAnswersCount++;

			totalPoints += pointsForThisQuestion;

			isTimerStarted = false;
			TimerTxt.gameObject.SetActive(false);
			spentTime += timeLimit - timeLeft;
		} else {
			CWPopUpText.text = "Wrong Answer !";
			CWPopUpPanel.transform.GetChild(2).gameObject.SetActive(false);
			CWPopUpPanel.transform.GetChild(3).gameObject.SetActive(true);
		}

		CWPopUpPanel.gameObject.SetActive(true);
	}
	private void SetCongratsPopUpPanel() {
		isTimerStarted = false;
		TimerTxt.gameObject.SetActive(false);
		CongratsPopUpText.text = "Correct answers: " + correctAnswersCount;
		CongratsPopUpPanel.gameObject.SetActive(true);
	}

	public void SubmitData() {
		Debug.Log("spentTime: " + spentTime);
		Debug.Log("correctAnswersCount: " + correctAnswersCount);
		Debug.Log("pointsCount: " + totalPoints);

		StartCoroutine(SendData());

		//LevelData level = new LevelData();
		//level.level = 0;
		//UpdateProgressMap(level);
	}
	public IEnumerator SendData() {

		UnityWebRequest web = UnityWebRequest.Get("http://192.168.80.132:5000/predictLevel?&time=" + spentTime + "&TotalPoints=" + totalPoints);
		
		web.method = "GET";
		yield return web.SendWebRequest();

		if (web.result == UnityWebRequest.Result.ConnectionError || web.responseCode != 200)
			Debug.LogError(web.error);
		else {
			

			LevelData level = new LevelData();
			level = JsonUtility.FromJson<LevelData>(web.downloadHandler.text);
			UpdateProgressMap(level);
			
			Debug.Log(level.level+1);
		}

		web.Dispose();

	}


	#endregion

	private void UpdateProgressMap(LevelData level) {

		for (int i = 0; i < ProgressImgArray.Length; i++) {
			ProgressImgArray[i].gameObject.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
		}

		for (int i = 0; i < ProgressImgArray.Length; i++) {
			if (i <= level.level) { //sets the color of the image component attached to the current element to an opaque white color (RGBA: 1, 1, 1, 1).
				ProgressImgArray[i].gameObject.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
			} else { //sets the color of the image component to a semi-transparent white color (RGBA: 1, 1, 1, 0.5f).
				ProgressImgArray[i].gameObject.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
			}
		}

		if (currentLvelProgress == 0) {

			if (level.level == 0) {
				ProgressDiscription.text = "You are at the same level as your previous attempt, Let's try to move forward!";
			} else if (level.level == 1) {
				ProgressDiscription.text = "Congratulations! You just moved 1 level up!";
			} else if (level.level == 2) {
				ProgressDiscription.text = "Congratulations! You just moved 2 levels up!";
			} else if (level.level == 3) {
				ProgressDiscription.text = "Congratulations! You are showing great preformance";
			}

		} else if (currentLvelProgress == 1) {

			if (level.level == 0) {
				ProgressDiscription.text = "Oops! You just moved 1 level down, Let's try to do well next time!";
			} else if (level.level == 1) {
				ProgressDiscription.text = "You are at the same level as your previous attempt, Let's try to move forward!";
			} else if (level.level == 2) {
				ProgressDiscription.text = "Congratulations! You just moved 1 level up!";
			} else if (level.level == 3) {
				ProgressDiscription.text = "Congratulations! You just moved 2 levels up!";
			}

		} else if (currentLvelProgress == 2) {

			if (level.level == 0) {
				ProgressDiscription.text = "Oops! You are moving down, Let's try to move forward!";
			} else if (level.level == 1) {
				ProgressDiscription.text = "Oops! You just moved 1 level down, Let's try to do well next time!";
			} else if (level.level == 2) {
				ProgressDiscription.text = "You are at the same level as your previous attempt, Let's try to move forward!";
			} else if (level.level == 3) {
				ProgressDiscription.text = "Congratulations! You just moved 1 level up!";
			}

		} else if (currentLvelProgress == 3) {

			if (level.level == 0) {
				ProgressDiscription.text = "Oops! You are moving down, Let's try to move forward!";
			} else if (level.level == 1) {
				ProgressDiscription.text = "Oops! You are moving down, Let's try to move forward!";
			} else if (level.level == 2) {
				ProgressDiscription.text = "Oops! You just moved 1 level down, Let's try to do well next time!";
			} else if (level.level == 3) {
				ProgressDiscription.text = "You are at the same level as your previous attempt, Let's try to move forward!";
			}

		}


		currentLvelProgress = level.level;
		PlayerPrefs.SetInt("currentLvelProgress", currentLvelProgress);

		ProgressmapPanel.gameObject.SetActive(true);
	}

	public void ResetProgress() {
		LevelData level = new LevelData();
		level.level = 0;
		currentLvelProgress = 0;

		UpdateProgressMap(level);

	}


}


[Serializable]
public struct LevelData {
	[SerializeField] public int level;
}