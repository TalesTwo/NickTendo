using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Types
{
    /* ----------------- ACTIVITY TYPES ----------------- */
    
    /// <summary> Enum for various types of activities. </summary>
    public enum ActivityType
    {
        Reading,
        WorkOut,
        Academic,
        Walking,
        Meditation,
        Cleaning,
        Journaling,
        Learning,
        Chores,
        Volunteering,
        Other
    }

    /// <summary> Enum for activity status. </summary>
    public enum ActivityStatus
    {
        Inactive,
        InProgress,
        Success,
        Failed
    }

    /// <summary> Struct for storing activity information. </summary>
    public struct ActivityInfo
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public string ActivityInstructions { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
        public ActivityType ActivityType { get; set; }
        public ActivityRewards ActivityRewards { get; set; }
        public float ActivityDuration { get; set; }

        /// <summary> Dumps the activity info for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Activity Info:\n" +
                      $"Name: {ActivityName}\n" +
                      $"Description: {ActivityDescription}\n" +
                      $"Instructions: {ActivityInstructions}\n" +
                      $"Status: {ActivityStatus}\n" +
                      $"Type: {ActivityType}\n" +
                      $"Rewards: {ActivityRewards.ToString()}\n" +
                      $"Duration: {ActivityDuration} seconds");
        }
    }

    /// <summary> Struct for storing activity rewards. </summary>
    public struct ActivityRewards
    {
        public float ExperiencePoints;

        /// <summary> Dumps the activity rewards for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Activity Rewards:\n" +
                      $"Experience Points: {ExperiencePoints}");
        }
    }

    /* ----------------- QUEST TYPES ----------------- */

    /// <summary> Enum for quest status. </summary>
    public enum QuestStatus
    {
        Inactive,
        InProgress,
        Completed
    }

    /// <summary> Struct for storing quest information. </summary>
    public struct QuestInfo
    {
        public string QuestName;
        public string QuestDescription;
        public string QuestInstructions;
        public QuestStatus QuestStatus;
        public ActivityType ActivityType;
        public QuestRewards QuestRewards;
        public float QuestDuration;

        /// <summary> Dumps the quest info for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Quest Info:\n" +
                      $"Name: {QuestName}\n" +
                      $"Description: {QuestDescription}\n" +
                      $"Instructions: {QuestInstructions}\n" +
                      $"Status: {QuestStatus}\n" +
                      $"Activity Type: {ActivityType}\n" +
                      $"Rewards: {QuestRewards.ToString()}\n" +
                      $"Duration: {QuestDuration} seconds");
        }
    }

    /// <summary> Struct for quest rewards. </summary>
    public struct QuestRewards
    {
        public float ExperiencePoints;
        public MedalInfo MedalInfo;

        /// <summary> Dumps the quest rewards for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Quest Rewards:\n" +
                      $"Experience Points: {ExperiencePoints}\n" +
                      $"Medal Info: {MedalInfo.ToString()}");
        }
    }

    /* ----------------- MEDAL TYPES ----------------- */

    /// <summary> Enum for medal types. </summary>
    public enum MedalType
    {
        Bronze,
        Silver,
        Gold,
        Diamond
    }

    /// <summary> Struct for storing medal information. </summary>
    public struct MedalInfo
    {
        public MedalType MedalType;
        public int Amount;

        /// <summary> Dumps the medal info for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Medal Info:\n" +
                      $"Type: {MedalType}\n" +
                      $"Amount: {Amount}");
        }
    }

    /* ----------------- PLAYER TYPES ----------------- */

    /// <summary> Enum for player gender. </summary>
    public enum PlayerGender
    {
        Male,
        Female,
        None
    }

    /// <summary> Enum for goal lengths. </summary>
    public enum GoalLengths
    {
        Short,
        Medium,
        Long
    }

    /// <summary> Struct for player preferences. </summary>
    [Serializable]
    public struct PlayerPreferences
    {
        public List<ActivityType> PreferredActivities;
        public List<GoalLengths> PreferredGoalLengths;

        /// <summary> Dumps the player preferences for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Player Preferences:\n" +
                      $"Preferred Activities: {string.Join(", ", PreferredActivities)}\n" +
                      $"Preferred Goal Lengths: {string.Join(", ", PreferredGoalLengths)}");
        }
    }

    /// <summary> Struct for storing player information. </summary>
    [Serializable]
    public struct PlayerInfo
    {
        public string name;
        public PlayerGender gender;
        public int level;
        public float currentExp;
        public float levelExp;
        public PlayerPreferences preferences;

        /// <summary> Dumps the player info for debugging purposes. </summary>
        public void Dump()
        {
            Debug.Log($"Player Info:\n" +
                      $"Name: {name}\n" +
                      $"Gender: {gender}\n" +
                      $"Level: {level}\n" +
                      $"Current Experience: {currentExp}\n" +
                      $"Experience to Level Up: {levelExp}\n" +
                      $"Preferences: {preferences.ToString()}");
        }
    }

    /* ----------------- UTILITY TYPES ----------------- */

}
