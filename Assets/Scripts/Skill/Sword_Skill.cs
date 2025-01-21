using System;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;

    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot unlockSwordBotton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("passive skills")]
    [SerializeField] private UI_SkillTreeSlot unlockTimeStopBotton;
    public bool timeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot unlockVolnurableBotton;
    public bool volnurableUnlocked;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot unlockBounceSwordBotton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot unlockPierceSwordBotton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot unlockSpinSwordBotton;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private float spinDuration;
    [SerializeField] private float spinGravity;
    [SerializeField] private float hitCooldown;

    private Vector2 finalDir;

    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        unlockSwordBotton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        unlockTimeStopBotton.GetComponent<Button>().onClick.AddListener(UnlockTimeStop);
        unlockVolnurableBotton.GetComponent<Button>().onClick.AddListener(UnlockVolnurable);
        unlockBounceSwordBotton.GetComponent<Button>().onClick.AddListener(UnlockBounceSword);
        unlockPierceSwordBotton.GetComponent<Button>().onClick.AddListener(UnlockPierceSword);
        unlockSpinSwordBotton.GetComponent<Button>().onClick.AddListener(UnlockSpinSword);

        GenereateDots();

        SetUpGravity();
    }

    protected override void Update()
    {
        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            finalDir = new Vector2(AimDirection().normalized.x*launchForce.x, AimDirection().normalized.y*launchForce.y);
        }


        if(Input.GetKey(KeyCode.Mouse1))
        {
            for(int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = DotsPosition(i*spaceBetweenDots);
            }
        }
    }


    #region ½âËø¼¼ÄÜ

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockTimeStop();
        UnlockVolnurable();
        UnlockBounceSword();
        UnlockPierceSword();
        UnlockSpinSword();
    }

    private void UnlockSword()
    {
        if (unlockSwordBotton.unlocked)
        {
            swordType = SwordType.Regular;
            swordUnlocked = true;
        }
    }

    private void UnlockTimeStop()
    {
        if (unlockTimeStopBotton.unlocked)
            timeStopUnlocked = true;
    }

    private void UnlockVolnurable()
    {
        if (unlockVolnurableBotton.unlocked)
            volnurableUnlocked = true;
    }

    private void UnlockBounceSword()
    {
        if (unlockBounceSwordBotton.unlocked)
            swordType = SwordType.Bounce;
    }

    private void UnlockPierceSword()
    {
        if (unlockPierceSwordBotton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSpinSword()
    {
        if (unlockSpinSwordBotton.unlocked)
            swordType = SwordType.Spin;
    }

    #endregion

    private void SetUpGravity()
    {
        if (swordType == SwordType.Bounce)
        {
            swordGravity = bounceGravity;
        }
        else if(swordType == SwordType.Pierce)
        {
            swordGravity = pierceGravity;
        }
        else if (swordType == SwordType.Spin)
        {
            swordGravity = spinGravity;
        }
    }


    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position,transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
        {
            newSwordScript.SetupBounce(true, bounceAmount,bounceSpeed);
        }

        else if(swordType == SwordType.Pierce)
        {
            newSwordScript.SetupPierce(pierceAmount);
        }

        else if(swordType == SwordType.Spin)
        {
            newSwordScript.SetupSpin(true,maxTravelDistance,spinDuration,hitCooldown);
        }


        newSwordScript.SetupSword(finalDir, swordGravity,player,freezeTimeDuration,returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }


    #region Aim region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directiom = mousePosition - playerPosition;

        return directiom;
    }

    public void DotsActive(bool _isActive)
    {
        for(int i = 0;i < dots.Length;i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private void GenereateDots()
    {
        dots = new GameObject[numberOfDots];
        for(int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab,player.transform.position,Quaternion.identity,dotsParent);

            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        Vector2 position = (Vector2)player.transform.position + new Vector2(
            AimDirection().normalized.x*launchForce.x,
            AimDirection().normalized.y*launchForce.y)*t + .5f*(Physics2D.gravity*swordGravity)*(t*t);

        return position;
    }
    #endregion
}
