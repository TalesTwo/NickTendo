using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    [System.Serializable]
    public class MovementLocation
    {
        public string Name;
        public Vector2 Position;
        public float TimeToReach;
    }


    [SerializeField]
    private GameObject _launcher;
    [SerializeField]
    private GameObject _friendFinderIcon;
    [SerializeField]
    private GameObject _deletePopUp;
    [SerializeField]
    private GameObject _dropDownMenu;
    [SerializeField]
    private GameObject _dropDownMenuSelectImage;
    [SerializeField]
    private GameObject _discordNotification;
    [SerializeField]
    private GameObject _friendRequest;
    [SerializeField]
    private GameObject _fakeCursor;
    [SerializeField]
    private Image _fader;
    [SerializeField]
    private MovementLocation[] _locations;


    private RectTransform _fCursorTransform;

    private void Start()
    {
        AudioManager.Instance.PlayCreditsTrack(1f, true, 0.1f, true, 0.1f);
        _fCursorTransform = _fakeCursor.GetComponent<RectTransform>();
        _fCursorTransform.localPosition = Vector2.zero;
        Invoke(nameof(Step1), 4f);
    }

    private void ClickSound()
    {
        AudioManager.Instance.PlayUISelectSound(0.5f);
    }

    private void Step1()
    {

        StartCoroutine(GoFromStartToFriendFinder());
    }

    IEnumerator GoFromStartToFriendFinder()
    {
        float _moveTime = 0;
        float _finishTime = _locations[1].TimeToReach;
        float _currentX = _locations[0].Position.x;
        float _currentY = _locations[0].Position.y;
        float _endX = _locations[1].Position.x;
        float _endY = _locations[1].Position.y;

        while (_moveTime < _finishTime)
        {
            _moveTime += Time.deltaTime;
            _fCursorTransform.localPosition = new Vector2(_currentX, _currentY);
            float _lerpValue = _moveTime / _finishTime;
            _currentX = Mathf.Lerp(_locations[0].Position.x, _endX, _lerpValue);
            _currentY = Mathf.Lerp(_locations[0].Position.y, _endY, _lerpValue);

            yield return null;
        }

        Invoke(nameof(ClickSound), 0.1f);
        Invoke(nameof(ActivateDropDown), 0.1f);
        Invoke(nameof(Step2), 1f);
    }

    private void ActivateDropDown()
    {
        _dropDownMenu.SetActive(true);
    }

    private void Step2()
    {
        StartCoroutine(GoFromFriendFinderToDeleteButton());
    }

    IEnumerator GoFromFriendFinderToDeleteButton()
    {
        float _moveTime = 0;
        float _finishTime = _locations[2].TimeToReach;
        float _currentX = _locations[1].Position.x;
        float _currentY = _locations[1].Position.y;
        float _endX = _locations[2].Position.x;
        float _endY = _locations[2].Position.y;

        while (_moveTime < _finishTime)
        {
            _moveTime += Time.deltaTime;
            _fCursorTransform.localPosition = new Vector2(_currentX, _currentY);
            float _lerpValue = _moveTime / _finishTime;
            _currentX = Mathf.Lerp(_locations[1].Position.x, _endX, _lerpValue);
            _currentY = Mathf.Lerp(_locations[1].Position.y, _endY, _lerpValue);

            yield return null;
        }

        Invoke(nameof(ClickSound), 0.1f);
        Invoke(nameof(ActivateDeletePopUp), 0.1f);
        Invoke(nameof(Step3), 1f);
    }

    private void ActivateDeletePopUp()
    {
        _deletePopUp.SetActive(true);
    }

    private void Step3()
    {
        StartCoroutine(GoFromDeleteButtonToYesDelete());
    }

    IEnumerator GoFromDeleteButtonToYesDelete()
    {
        float _moveTime = 0;
        float _finishTime = _locations[3].TimeToReach;
        float _currentX = _locations[2].Position.x;
        float _currentY = _locations[2].Position.y;
        float _endX = _locations[3].Position.x;
        float _endY = _locations[3].Position.y;

        while (_moveTime < _finishTime)
        {
            _moveTime += Time.deltaTime;
            _fCursorTransform.localPosition = new Vector2(_currentX, _currentY);
            float _lerpValue = _moveTime / _finishTime;
            _currentX = Mathf.Lerp(_locations[2].Position.x, _endX, _lerpValue);
            _currentY = Mathf.Lerp(_locations[2].Position.y, _endY, _lerpValue);

            yield return null;
        }

        Invoke(nameof(ClickSound), 0.1f);
        Invoke(nameof(DeleteFrinedFinder), 0.2f);
        Invoke(nameof(NotificationSound), 0.5f);
        Invoke(nameof(Step4), 2);
    }

    private void NotificationSound()
    {
        AudioManager.Instance.PlayNotificationSound(0.5f);
        _discordNotification.SetActive(true);
    }

    private void DeleteFrinedFinder()
    {
        AudioManager.Instance.PlayDeleteAppSound(0.5f);
        _deletePopUp.SetActive(false);
        _dropDownMenu.SetActive(false);
        _launcher.SetActive(false);
        _friendFinderIcon.SetActive(false);
    }

    private void Step4()
    {
        StartCoroutine(GoFromYesDeleteToNotification());
    }

    IEnumerator GoFromYesDeleteToNotification()
    {
        float _moveTime = 0;
        float _finishTime = _locations[4].TimeToReach;
        float _currentX = _locations[3].Position.x;
        float _currentY = _locations[3].Position.y;
        float _endX = _locations[4].Position.x;
        float _endY = _locations[4].Position.y;

        while (_moveTime < _finishTime)
        {
            _moveTime += Time.deltaTime;
            _fCursorTransform.localPosition = new Vector2(_currentX, _currentY);
            float _lerpValue = _moveTime / _finishTime;
            _currentX = Mathf.Lerp(_locations[3].Position.x, _endX, _lerpValue);
            _currentY = Mathf.Lerp(_locations[3].Position.y, _endY, _lerpValue);

            yield return null;
        }

        Invoke(nameof(ClickSound), 0.1f);
        Invoke(nameof(RemoveNotification), 0.1f);
        Invoke(nameof(Step5), 1f);
    }

    private void RemoveNotification()
    {
        _discordNotification.SetActive(false);
        _friendRequest.SetActive(true);
    }

    private void Step5()
    {
        StartCoroutine(GoFromNotificationToAccept());
    }

    IEnumerator GoFromNotificationToAccept()
    {
        float _moveTime = 0;
        float _finishTime = _locations[5].TimeToReach;
        float _currentX = _locations[4].Position.x;
        float _currentY = _locations[4].Position.y;
        float _endX = _locations[5].Position.x;
        float _endY = _locations[5].Position.y;

        while (_moveTime < _finishTime)
        {
            _moveTime += Time.deltaTime;
            _fCursorTransform.localPosition = new Vector2(_currentX, _currentY);
            float _lerpValue = _moveTime / _finishTime;
            _currentX = Mathf.Lerp(_locations[4].Position.x, _endX, _lerpValue);
            _currentY = Mathf.Lerp(_locations[4].Position.y, _endY, _lerpValue);

            yield return null;
        }

        Invoke(nameof(ClickSound), 0.1f);
        Invoke(nameof(Step6), 0.5f);
    }

    private void Step6()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float _effectTime = 0;
        Color _newColor = _fader.color;
        _newColor.a = 0;
        while(_effectTime < 5)
        {
            _effectTime += Time.deltaTime;
            _fader.color = _newColor;
            float _lerpValue = _effectTime / 5;
            _newColor.a = Mathf.Lerp(0, 1, _lerpValue);

            yield return null;
        }

        StartCredits();
    }

    private void StartCredits()
    {
        CreditsManager.Instance.BeginCredits();
    }
}