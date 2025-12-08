using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapMoverUI : MonoBehaviour
{
    private enum Directions
    {
        right,
        left,
        up,
        down
    }

    [SerializeField]
    private float _xMovementRate;
    [SerializeField]
    private float _yMovementRate;
    [SerializeField]
    private RectTransform _scrollArenaRect;

    private (int x, int y) _minimapVisualCoords;
    private bool _isCalibrating;

    private void Start()
    {
        _minimapVisualCoords = (0, 0);
        _isCalibrating = true;
        EventBroadcaster.PlayerChangedRoom += OnPlayerChangedRoom;
        EventBroadcaster.GameStarted += GameStarted;
    }

    private void GameStarted()
    {
        /*(int x, int y)  _startCoords= DungeonController.Instance.GetCurrentRoomCoords();
        DebugUtils.Log($"Start X/row: {_startCoords.x} and Start Y/col: {_startCoords.y}");

        while (_minimapVisualCoords.x != _startCoords.x)
        {
            DebugUtils.Log($"X/row: {_minimapVisualCoords.x} and Y/col: {_minimapVisualCoords.y}");
            if (_startCoords.x > _minimapVisualCoords.x)
            {
                _minimapVisualCoords.x++;
                MoveMinimap(Directions.left);
            }
            else if (_startCoords.x < _minimapVisualCoords.x)
            {
                _minimapVisualCoords.x--;
                MoveMinimap(Directions.right);
            }
        }

        while (_minimapVisualCoords.y != _startCoords.y)
        {
            DebugUtils.Log($"X/row: {_minimapVisualCoords.x} and Y/col: {_minimapVisualCoords.y}");
            if (_startCoords.y  > _minimapVisualCoords.y)
            {
                _minimapVisualCoords.y++;
                MoveMinimap(Directions.down);
            }
            else if (_startCoords.y < _minimapVisualCoords.y)
            {
                _minimapVisualCoords.y--;
                MoveMinimap(Directions.up);
            }
        }*/

    }

    private void OnPlayerChangedRoom((int y, int x) targetRoomCoords)
    {
        DebugUtils.Log($"New X/row: {targetRoomCoords.x} and new Y/col: {targetRoomCoords.y}");

        if(targetRoomCoords.x > _minimapVisualCoords.x)
        {
            _minimapVisualCoords.x++;
            MoveMinimap(Directions.left);
        }
        else if (targetRoomCoords.x < _minimapVisualCoords.x)
        {
            _minimapVisualCoords.x--;
            MoveMinimap(Directions.right);
        }
        else if (targetRoomCoords.y  > _minimapVisualCoords.y)
        {
            _minimapVisualCoords.y++;
            MoveMinimap(Directions.down);
        }
        else if (targetRoomCoords.y < _minimapVisualCoords.y)
        {
            _minimapVisualCoords.y--;
            MoveMinimap(Directions.up);
        }
        DebugUtils.Log($"X/row: {_minimapVisualCoords.x} and Y/col: {_minimapVisualCoords.y}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            MoveMinimap(Directions.up);
        }
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            MoveMinimap(Directions.down);
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            MoveMinimap(Directions.right);
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            MoveMinimap(Directions.left);
        }


        if (Input.GetKeyDown(KeyCode.O))
        {
            (int x, int y) _startCoords = DungeonController.Instance.GetCurrentRoomCoords();
            DebugUtils.Log($"Start X/row: {_startCoords.x} and Start Y/col: {_startCoords.y}");

            while (_minimapVisualCoords.x != _startCoords.x)
            {
                if (_startCoords.x > _minimapVisualCoords.x)
                {
                    _minimapVisualCoords.x++;
                    MoveMinimap(Directions.left);
                }
                else if (_startCoords.x < _minimapVisualCoords.x)
                {
                    _minimapVisualCoords.x--;
                    MoveMinimap(Directions.right);
                }
            }

            while (_minimapVisualCoords.y != _startCoords.y)
            {
                if (_startCoords.y > _minimapVisualCoords.y)
                {
                    _minimapVisualCoords.y++;
                    MoveMinimap(Directions.down);
                }
                else if (_startCoords.y < _minimapVisualCoords.y)
                {
                    _minimapVisualCoords.y--;
                    MoveMinimap(Directions.up);
                }
            }
            DebugUtils.Log($"X/row: {_minimapVisualCoords.x} and Y/col: {_minimapVisualCoords.y}");
        }

    }

    private void MoveMinimap(Directions direction)
    {
        if (direction == Directions.left)
        {
            float newX = _scrollArenaRect.localPosition.x - _xMovementRate;
            _scrollArenaRect.localPosition = new Vector2(newX, _scrollArenaRect.localPosition.y);
        }
        else if (direction == Directions.right)
        {
            float newX = _scrollArenaRect.localPosition.x + _xMovementRate;
            _scrollArenaRect.localPosition = new Vector2(newX, _scrollArenaRect.localPosition.y);
        }
        else if (direction == Directions.up)
        {
            float newY = _scrollArenaRect.localPosition.y + _yMovementRate;
            _scrollArenaRect.localPosition = new Vector2(_scrollArenaRect.localPosition.x, newY);
        }
        else if (direction == Directions.down)
        {
            float newY = _scrollArenaRect.localPosition.y - _yMovementRate;
            _scrollArenaRect.localPosition = new Vector2(_scrollArenaRect.localPosition.x, newY);
        }
    }
}