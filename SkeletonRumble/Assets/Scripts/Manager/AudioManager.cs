using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Valis wunderschönes: <3

    [Header("Testing")]
    public float testTimer;
    private float resetTestTimer;
    public float testTimer2;
    private float resetTestTimer2;

    [System.Serializable]
    public struct AudioClipPackage
    {
        public AudioClip[] soundClips;

        [Range(-3, 0)] public float soundClips_MinPitch;
        [Range(0, 3)] public float soundClips_MaxPitch;
    }
    [System.Serializable]
    public struct AudioPackage
    {
        public AudioSource[] audioSources;
        public AudioClipPackage[] clipSettings;
    }



    [Header("Footsteps")]
    public AudioPackage footsteps;

    [Header("Jump")]
    public AudioPackage jump;

    [Header("Land")]
    public AudioPackage land;

    [Header("Dash")]
    public AudioPackage dash;

    [Header("Wallslide")]
    public AudioPackage wallslide;

    [Header("Attack")]
    public AudioPackage attack;

    [Header("Damage")]
    public AudioPackage damage;

    [Header("Parry")]
    public AudioPackage parry;



    //----------------------------------------

    #region Testing
    //private void Start()
    //{
    //    resetTestTimer = testTimer;
    //    resetTestTimer2 = testTimer2;
    //}

    //private void Update()
    //{
    //    testTimer -= Time.deltaTime;

    //    if (testTimer <= 0)
    //    {
    //        testTimer = resetTestTimer;


    //        //PlayFootstepSounds(0, true);
    //        //PlayWallslideSound(0, true);
    //        //PlayDashSound(0);
    //        //PlayJumpSound(0, true);
    //        //PlayLandSound(0, true);
    //        //PlayAttackSound(0, true);
    //        //PlayDamageSound(0, true);
    //        PlayParrySound(0, true);
    //    }

    //    testTimer2 -= Time.deltaTime;

    //    if (testTimer2 <= 0)
    //    {
    //        testTimer2 = resetTestTimer2;


    //        //PlayFootstepSounds(1, false);
    //        //PlayWallslideSound(1, false);
    //        //PlayJumpSound(1, false);
    //        //PlayLandSound(1, false);
    //        //PlayAttackSound(1, false);
    //        //PlayDamageSound(1, false);
    //        PlayDamageSound(1, false);
    //    }
    //}
    #endregion

    #region Funktions To Trigger Sounds
    public void PlayFootstepSound(int indexOfCharacter, bool trueEqualsGrass_FalseEqualsStone)
    {
        if (trueEqualsGrass_FalseEqualsStone)
        {
            //Grass
            this.footsteps.audioSources[indexOfCharacter].clip = this.footsteps.clipSettings[0].soundClips[Random.Range(0, this.footsteps.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.footsteps.audioSources[indexOfCharacter], this.footsteps.clipSettings[0].soundClips_MinPitch, this.footsteps.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Stone
            this.footsteps.audioSources[indexOfCharacter].clip = this.footsteps.clipSettings[1].soundClips[Random.Range(0, this.footsteps.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.footsteps.audioSources[indexOfCharacter], this.footsteps.clipSettings[1].soundClips_MinPitch, this.footsteps.clipSettings[1].soundClips_MaxPitch);
        }

        this.footsteps.audioSources[indexOfCharacter].Play();
    }

    public void PlayWallslideSound(int indexOfCharacter, bool trueEqualsStart_FalseEqualsStop)
    {
        if (trueEqualsStart_FalseEqualsStop)
        {
            this.wallslide.audioSources[indexOfCharacter].clip = this.wallslide.clipSettings[0].soundClips[Random.Range(0, this.wallslide.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.wallslide.audioSources[indexOfCharacter], this.wallslide.clipSettings[0].soundClips_MinPitch, this.wallslide.clipSettings[0].soundClips_MaxPitch);

            this.wallslide.audioSources[indexOfCharacter].Play();
        }
        else
        {
            this.wallslide.audioSources[indexOfCharacter].Stop();
        }
    }

    public void PlayDashSound(int indexOfCharacter)
    {
        this.dash.audioSources[indexOfCharacter].clip = this.dash.clipSettings[0].soundClips[Random.Range(0, this.dash.clipSettings[0].soundClips.Length)];
        this.PitchSound(this.dash.audioSources[indexOfCharacter], this.dash.clipSettings[0].soundClips_MinPitch, this.dash.clipSettings[0].soundClips_MaxPitch);

        this.dash.audioSources[indexOfCharacter].Play();
    }

    public void PlayJumpSound(int indexOfCharacter, bool trueEqualsGrass_FalseEqualsStone)
    {
        if (trueEqualsGrass_FalseEqualsStone)
        {
            //Grass
            this.jump.audioSources[indexOfCharacter].clip = this.jump.clipSettings[0].soundClips[Random.Range(0, this.jump.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.jump.audioSources[indexOfCharacter], this.jump.clipSettings[0].soundClips_MinPitch, this.jump.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Stone
            this.jump.audioSources[indexOfCharacter].clip = this.jump.clipSettings[1].soundClips[Random.Range(0, this.jump.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.jump.audioSources[indexOfCharacter], this.jump.clipSettings[1].soundClips_MinPitch, this.jump.clipSettings[1].soundClips_MaxPitch);
        }

        this.jump.audioSources[indexOfCharacter].Play();
    }

    public void PlayLandSound(int indexOfCharacter, bool trueEqualsGrass_FalseEqualsStone)
    {
        if (trueEqualsGrass_FalseEqualsStone)
        {
            //Grass
            this.land.audioSources[indexOfCharacter].clip = this.land.clipSettings[0].soundClips[Random.Range(0, this.land.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.land.audioSources[indexOfCharacter], this.land.clipSettings[0].soundClips_MinPitch, this.land.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Stone
            this.land.audioSources[indexOfCharacter].clip = this.land.clipSettings[1].soundClips[Random.Range(0, this.land.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.land.audioSources[indexOfCharacter], this.land.clipSettings[1].soundClips_MinPitch, this.land.clipSettings[1].soundClips_MaxPitch);
        }

        this.land.audioSources[indexOfCharacter].Play();
    }

    public void PlayAttackSound(int indexOfCharacter, bool trueEqualsSword_FalseEqualsHammer)
    {
        if (trueEqualsSword_FalseEqualsHammer)
        {
            //Sword
            this.attack.audioSources[indexOfCharacter].clip = this.attack.clipSettings[0].soundClips[Random.Range(0, this.attack.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.attack.audioSources[indexOfCharacter], this.attack.clipSettings[0].soundClips_MinPitch, this.attack.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Hammer
            this.attack.audioSources[indexOfCharacter].clip = this.attack.clipSettings[1].soundClips[Random.Range(0, this.attack.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.attack.audioSources[indexOfCharacter], this.attack.clipSettings[1].soundClips_MinPitch, this.attack.clipSettings[1].soundClips_MaxPitch);
        }

        this.attack.audioSources[indexOfCharacter].Play();
    }

    public void PlayDamageSound(int indexOfCharacter, bool trueEqualsSword_FalseEqualsHammer)
    {
        if (trueEqualsSword_FalseEqualsHammer)
        {
            //Sword
            this.damage.audioSources[indexOfCharacter].clip = this.damage.clipSettings[0].soundClips[Random.Range(0, this.damage.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.damage.audioSources[indexOfCharacter], this.damage.clipSettings[0].soundClips_MinPitch, this.damage.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Hammer
            this.damage.audioSources[indexOfCharacter].clip = this.damage.clipSettings[1].soundClips[Random.Range(0, this.damage.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.damage.audioSources[indexOfCharacter], this.damage.clipSettings[1].soundClips_MinPitch, this.damage.clipSettings[1].soundClips_MaxPitch);
        }

        this.damage.audioSources[indexOfCharacter].Play();
    }

    public void PlayParrySound(int indexOfCharacter, bool trueEqualsSword_FalseEqualsHammer)
    {
        if (trueEqualsSword_FalseEqualsHammer)
        {
            //Sword
            this.parry.audioSources[indexOfCharacter].clip = this.parry.clipSettings[0].soundClips[Random.Range(0, this.parry.clipSettings[0].soundClips.Length)];
            this.PitchSound(this.parry.audioSources[indexOfCharacter], this.parry.clipSettings[0].soundClips_MinPitch, this.parry.clipSettings[0].soundClips_MaxPitch);
        }
        else
        {
            //Hammer
            this.parry.audioSources[indexOfCharacter].clip = this.parry.clipSettings[1].soundClips[Random.Range(0, this.parry.clipSettings[1].soundClips.Length)];
            this.PitchSound(this.parry.audioSources[indexOfCharacter], this.parry.clipSettings[1].soundClips_MinPitch, this.parry.clipSettings[1].soundClips_MaxPitch);
        }

        this.parry.audioSources[indexOfCharacter].Play();
    }
    #endregion

    #region Effekts For Sounds
    private void PitchSound(AudioSource sourceToPitch, float minPitch, float maxPitch)
    {
        sourceToPitch.pitch = 1 + Random.Range(minPitch, maxPitch);
    }
    #endregion
}
