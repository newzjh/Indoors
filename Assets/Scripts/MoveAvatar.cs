﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class MoveAvatar : MonoBehaviour {

	public GameObject avatarFigure;

	public AvatarAnimationState animationState = AvatarAnimationState.Idle;
	[HideInInspector] public float dist;
	public enum AvatarAnimationState{
		Idle, 
		Walk,
		Run
	};
	public GOAvatarAnimationStateEvent OnAnimationStateChanged;
	public UnityEvent<Vector3> OnAvatarPositionChanged;

	// Use this for initialization
	void Start () {

        //goMap.locationManager.onOriginSet.AddListener((Coordinates) => {OnOriginSet(Coordinates); OnLocationChanged(Coordinates); });
        //goMap.locationManager.onLocationChanged.AddListener((Coordinates) => {OnLocationChanged(Coordinates);});
        //if (goMap.useElevation)
        //	goMap.OnTileLoad.AddListener((GOTile) => {OnTileLoad(GOTile);});

        OnAvatarPositionChanged.Invoke(transform.position);
    }


	#region Location manager events





    public Action centerTileChanged;

	//void OnLocationChanged (Coordinates currentLocation) {

	//	Vector3 lastPosition = transform.position;

	//	//Position
	//	Vector3 currentPosition = currentLocation.convertCoordinateToVector (0);

	//	if(goMap.useElevation)
	//		currentPosition = GOMap.AltitudeToPoint (currentPosition);

	//	if (lastPosition == Vector3.zero) {
	//		lastPosition = currentPosition;
	//	}
			
	//	moveAvatar (lastPosition,currentPosition);


 //       currentTileCenter = currentLocation.tileCenter(goMap.zoomLevel);

	//	if (currentTileCenter==null || lastTileCenter == null || (currentTileCenter != null && lastTileCenter!=null && !currentTileCenter.NearlyEquals(lastTileCenter)))
 //       {
	//		OnCenterTileChanged();
	//		if (centerTileChanged != null)
	//			centerTileChanged();
 //           lastTileCenter = currentTileCenter;
	//	}
		
	//}

	public void OnCenterTileChanged()
	{

	}

	#endregion

	#region Move Avatar

	public void moveAvatar(float x,float y)
    {
		Vector3 lastPosition = transform.position;

		//Position
		Vector3 currentPosition = transform.position;
		currentPosition.x += x;
		currentPosition.z += y;

		if (lastPosition == Vector3.zero)
		{
			lastPosition = currentPosition;
		}

		moveAvatar(lastPosition, currentPosition);
	}

	private Coroutine coroutine;

	public async void moveAvatar (Vector3 lastPosition, Vector3 currentPosition) {

		await move (lastPosition,currentPosition,0.5f);
	}

	private async UniTask move(Vector3 lastPosition, Vector3 currentPosition, float time) {

		float elapsedTime = 0;
		Vector3 targetDir = currentPosition-lastPosition;
		Quaternion finalRotation = avatarFigure.transform.rotation;
		if (targetDir.magnitude>0.0f)
			finalRotation = Quaternion.LookRotation (targetDir);

		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(lastPosition, currentPosition, (elapsedTime / time));
			avatarFigure.transform.rotation = Quaternion.Lerp(avatarFigure.transform.rotation, finalRotation,(elapsedTime / time));

			elapsedTime += Time.deltaTime;

			dist = Vector3.Distance (lastPosition, currentPosition);

            OnAvatarPositionChanged.Invoke(transform.position);

			AvatarAnimationState state = AvatarAnimationState.Idle; 

			if (dist > 4)
				state = AvatarAnimationState.Run;
			else state = AvatarAnimationState.Walk;

			if (state != animationState) {

				animationState = state;
				OnAnimationStateChanged.Invoke(animationState);
			}

			await UniTask.WaitForEndOfFrame();
		}

		animationState = AvatarAnimationState.Idle;
		OnAnimationStateChanged.Invoke(animationState);

    }
		
	void rotateAvatar(Vector3 lastPosition) {

		Vector3 targetDir = transform.position-lastPosition;

		if (targetDir != Vector3.zero) {
			avatarFigure.transform.rotation = Quaternion.Slerp(
				avatarFigure.transform.rotation,
				Quaternion.LookRotation(targetDir),
				Time.deltaTime * 10.0f
			);
		}
	}

	#endregion
}

[Serializable]
public class GOAvatarAnimationStateEvent : UnityEvent <MoveAvatar.AvatarAnimationState> {


}