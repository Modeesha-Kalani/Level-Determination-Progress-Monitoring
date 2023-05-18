
using System;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "ImageAndNameData", menuName = "ScriptableObjects/ImageAndNameData")]
public class ImageAndNameData : ScriptableObject { //create scriptable objects that contain an array of ImageAndName objects
    
    public ImageAndName[] mImageAndNameDataSet;
    
}

[Serializable]
public class ImageAndName { //one object of this class has a name,image and an audio clip
    public string mName;
    public Sprite mImage;
    public AudioClip mAudioClip;
}


