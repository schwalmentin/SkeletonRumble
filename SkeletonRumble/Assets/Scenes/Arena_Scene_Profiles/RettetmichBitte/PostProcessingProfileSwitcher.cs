using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessingProfileSwitcher : MonoBehaviour
{
    public float lerpSpeed;
    public PostProcessVolume[] volumes;
    public SkinnedMeshRenderer[] enemyClothes;
    public MeshRenderer[] enemyWeapons;
    public ParticleSystem[] enemyParticles;
    public EmotionColors emotionColors;
    public Light enemyLight;
    private float currentWeight = 1;
    private Profiles profileState;

    //Characters
    private PlayerCharacter playerCharacter;
    private EnemyController enemyCharacter;

    //Evaluation of Profiles
    [Range(1, 2)] public float aboveLevelPercentage;
    [Range(0, 1)] public float onSameLevelPercentage;
    [Range(0, 1)] public float intensePercentage;

    private PostProcessVolume newVolume;
    private PostProcessVolume currentVolume;

    private bool change;
    [HideInInspector] public bool fighting;

    private void Start()
    {
        this.profileState = Profiles.Normal;

        this.change = false;
        this.currentVolume = this.volumes[0];
        this.newVolume = this.volumes[0];

        this.playerCharacter = FindObjectOfType<PlayerCharacter>();
        this.enemyCharacter = FindObjectOfType<EnemyController>();
    }

    private void Update()
    {
        this.ChangeColor();

        if (this.change)
        {
            this.change = !this.ChangeProfile(this.newVolume, this.currentVolume, this.lerpSpeed);
        }

        if (!this.fighting)
        {
            return;
        }

        this.EvaluateProfiles();
        if (this.change)
        {
            this.change = !this.ChangeProfile(this.newVolume, this.currentVolume, this.lerpSpeed);
        }
    }

    public void SwitchProfile(Profiles profileState)
    {
        switch (profileState)
        {
            case Profiles.Normal:

                if (this.profileState != Profiles.Normal)
                {
                    this.change = true;
                    this.currentVolume = this.newVolume;
                    this.newVolume = this.volumes[0];
                    this.profileState = Profiles.Normal;
                    this.currentWeight = 1;
                }
                break;

            case Profiles.Lose:

                if (this.profileState != Profiles.Lose)
                {
                    this.change = true;
                    this.currentVolume = this.newVolume;
                    this.newVolume = this.volumes[3];
                    this.profileState = Profiles.Lose;
                    this.currentWeight = 1;
                    print("looooooooooool");
                }
                break;

            case Profiles.Win:

                if (this.profileState != Profiles.Win)
                {
                    this.change = true;
                    this.currentVolume = this.newVolume;
                    this.newVolume = this.volumes[4];
                    this.profileState = Profiles.Win;
                    this.currentWeight = 1;
                }
                break;
        }
    }

    [Range(0, 100)] public int anger;
    [Range(0, 100)] public int elegance;
    [Range(0, 100)] public int arrogance;
    [Range(0, 100)] public int fear;

    private void ChangeColor()
    {
        //EmotionManager.Anger = this.anger;
        //EmotionManager.Elegance = this.elegance;
        //EmotionManager.Arrogance = this.arrogance;
        //EmotionManager.Fear = this.fear;

        float r = this.emotionColors.neutral.a * EmotionManager.TempNeutral / 100 +
            this.emotionColors.anger.a * EmotionManager.tempAnger / 100 +
            this.emotionColors.elegance.a * EmotionManager.tempElegance / 100 +
            this.emotionColors.arrogance.a * EmotionManager.tempArrogance / 100 +
            this.emotionColors.fear.a * EmotionManager.tempFear / 100;
        float g = this.emotionColors.neutral.g * EmotionManager.TempNeutral / 100 +
            this.emotionColors.anger.g * EmotionManager.tempAnger / 100 +
            this.emotionColors.elegance.g * EmotionManager.tempElegance / 100 +
            this.emotionColors.arrogance.g * EmotionManager.tempArrogance / 100 +
            this.emotionColors.fear.g * EmotionManager.tempFear / 100;
        float b = this.emotionColors.neutral.b * EmotionManager.TempNeutral / 100 +
            this.emotionColors.anger.b * EmotionManager.tempAnger / 100 +
            this.emotionColors.elegance.b * EmotionManager.tempElegance / 100 +
            this.emotionColors.arrogance.b * EmotionManager.tempArrogance / 100 +
            this.emotionColors.fear.b * EmotionManager.tempFear / 100;

        Color targetColor = new Color(r, g, b);

        //Clothes
        foreach (var item in this.enemyClothes)
        {
            item.material.color = targetColor;
            item.material.SetColor("_EmissionColor", targetColor);
        }

        //Weapons
        foreach (var item in this.enemyWeapons)
        {
            item.material.color = targetColor;
            item.material.SetColor("_EmissionColor", targetColor);
        }

        //Particles
        foreach (var item in this.enemyParticles)
        {
            ParticleSystem.MainModule ma = item.main;
            ma.startColor = targetColor;
        }

        //Light
        if (this.enemyLight != null)
        {
            switch (EmotionManager.GetEmotion())
            {
                case EmotionManager.CurrentEmotion.Neutral:
                    targetColor = this.emotionColors.neutral;
                    break;
                case EmotionManager.CurrentEmotion.Elegance:
                    targetColor = this.emotionColors.elegance;
                    break;
                case EmotionManager.CurrentEmotion.Arrogance:
                    targetColor = this.emotionColors.arrogance;
                    break;
                case EmotionManager.CurrentEmotion.Anger:
                    targetColor = this.emotionColors.anger;
                    break;
                case EmotionManager.CurrentEmotion.Fear:
                    targetColor = this.emotionColors.fear;
                    break;
                default:
                    break;
            }

            this.enemyLight.color = targetColor;
        }
    }

    private void EvaluateProfiles()
    {
        if (this.change)
        {
            return;
        }

        float playerHealthPercentage = this.playerCharacter.currentHealth / this.playerCharacter.maxHealth;
        float enemyHealthPercentage = this.enemyCharacter.currentHealth / this.enemyCharacter.maxHealth;

        //Hope
        if (enemyHealthPercentage * this.aboveLevelPercentage < playerHealthPercentage)
        {
            if (this.profileState != Profiles.Hope)
            {
                this.change = true;
                this.currentVolume = this.newVolume;
                this.newVolume = this.volumes[2];
                this.profileState = Profiles.Hope;
                this.currentWeight = 1;
            }
            //print("HOPE++++++++++++++++++++++");
            return;
        }

        //Intense
        if ((Mathf.Abs(enemyHealthPercentage - playerHealthPercentage) < this.onSameLevelPercentage && enemyHealthPercentage < this.intensePercentage && playerHealthPercentage < this.intensePercentage) ||
            playerHealthPercentage * this.aboveLevelPercentage < enemyHealthPercentage)
        {
            if (this.profileState != Profiles.Intense)
            {
                this.change = true;
                this.currentVolume = this.newVolume;
                this.newVolume = this.volumes[1];
                this.profileState = Profiles.Intense;
                this.currentWeight = 1;
            }
            //print("INTENSE------------------");
            return;
        }

        //Normal
        if (Mathf.Abs(enemyHealthPercentage - playerHealthPercentage) < this.onSameLevelPercentage && enemyHealthPercentage > this.intensePercentage && playerHealthPercentage > this.intensePercentage)
        {
            if (this.profileState != Profiles.Normal)
            {
                this.change = true;
                this.currentVolume = this.newVolume;
                this.newVolume = this.volumes[0];
                this.profileState = Profiles.Normal;
                this.currentWeight = 1;
            }
            //print("NORMAL~~~~~~~~~~~~~~~~~~~~~");
        }
    }

    private bool ChangeProfile(PostProcessVolume volumeToAktivate, PostProcessVolume volumeToDisable, float lerpSpeed)
    {
        this.currentWeight -= Time.deltaTime * lerpSpeed;

        volumeToAktivate.weight = Mathf.Lerp(0.7f, 0, this.currentWeight);
        volumeToDisable.weight = Mathf.Lerp(0, 0.7f, this.currentWeight);

        if (this.currentWeight <= 0)
        {
            this.currentWeight = 1;
            return true;
        }

        return false;       
    }

    [System.Serializable]
    public struct EmotionColors
    {
        public Color neutral;
        public Color anger;
        public Color elegance;
        public Color arrogance;
        public Color fear;
    }
}

public enum Profiles
{
    Normal,
    Intense,
    Hope,
    Lose,
    Win
}
