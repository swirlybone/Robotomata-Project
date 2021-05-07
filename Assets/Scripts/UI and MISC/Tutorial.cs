using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NotificationCenter;
using static MessageHelper;

public class Tutorial : MonoBehaviour
{
    bool ready = false;

    private void OnEnable()
    {
        AddObserver("DonePlacing", Done);
    }

    private void OnDisable()
    {
        RemoveObserver("DonePlacing", Done);
    }

    private void Start()
    {
        StartCoroutine(TutorialLoop());
    }

    IEnumerator TutorialLoop()
    {
        yield return new WaitForSeconds(1);
        Message("Hello and welcome, %GENDER_HUMAN_THINGY! You have been assigned to manage this fine warehouse-ian establishment place! Or whatever it is! First off, press Enter (not the keypad one, you ape) to go to the next message from the tutorial.cs script. I mean the uh, advice... I'm giving you. I'm real.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Just to get you situated with the controls, use WASD to move the camera, Arrow keys to rotate the camera. You can also hold Left Alt to move the camera with the mouse, and use Q and E for UP and DOWN");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("In this place, your 'Robotomatons' (Yea right? Try saying that twice) are going to do ALL your dirty work... of... moving boxes around? Is that really what game design is these days? Alright, whatever, I don't get paid to ask questions.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("So then, you need to get started somewhere. First, you have to purchase the Robots!");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Here is just enough money for you to buy some stuff without breaking the tutorial! This is enough for a single small Robotomaton. Unless you break the game, then maybe you can buy 10. Bonus: I'll hate you also.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        PostNotification("AwardMoney", 250);
        Message("Press whatever key is set to open the menu and click to buy a robot! Don't worry, that key is normally (R) (No, capslock doesn't matter. If it does... the devs have a bug to fix).");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Once you purchase a bot, LEFT click once to place them into the world, and then cry as you realize you've doomed them to a life of servitude!");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Just kidding, click at least once more to give them somewhere to walk to. Or click 50 times, it's your bot. You paid for the whole bot, you're USING the whole bot.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("After that, just RIGHT click once to let him go. But beware! We have to make this game hard, so the robot will eventually break down and require repairs (For money, obviously, you aren't important enough to get government subsidies.)");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        yield return null;
    }

    bool KeyDown() => Input.GetKeyDown(KeyCode.Return);

    void Done(Notification notification)
    {
        StartCoroutine(WaitForDonePlacing());
        if (ready)
        {
            PostNotification("AwardMoney", 1000);
            Message("Now that you've done this the game has started for REAL this time! Just wait until you see all the other nonsense mechanics we added to this game, OH BOY YOU'LL BE SORRY YOU BOUGHT THIS!! I'm just kidding, it's not that bad... Unless? o_o. Um, well, anyway here's $1000. Go have fun!");
            
            Destroy(gameObject);
        }
    }

    IEnumerator WaitForDonePlacing()
    {
        Message("After about 20 seconds your robot will break, and you must press (F) to pay respects... also to repair it (make sure you hover your mouse cursor over them first!)");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Now that you have a bot, you'll need some activities to fill their pathetic, eventless lives with.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        PostNotification("AwardMoney", 1000);
        Message("Here's enough to Open the Production outlet and pay your rent. Don't blow it on drugs, please.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Click the Production conveyor. It costs 500 to start up, and beware! It spends the money immediately with no possibility of refunds, because this is a capitalist utopia, not a customer utopia. Also, just so you're familiar with the controls, instead of pressing space to close this right now, use the button I put there. I spent a lot of time on it.                                                  That's a lie it's actually really easy to implement.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("Now if you didn't mess that up and force me to start from the beginning, you're ready to assign jobs to the bot you bought. Man that was an impossible clever rhyme, you probably didn't even see it coming. Friggin' owned.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("You must Re-path your robot by first hovering over them and pressing (T), then you will be allowed to place them down again for free. But before you do it! Bother reading these handy tutorial messages I wrote out for you!");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("In order to assign the robot a job, you must click on a Station. Be sure to click the operator box with the lever in order to assign the job for the labeler! The bot will pick up boxes off the floor automatically, but must be told what to do afterwards. One example is a station is the deposit you-bought-it. BOOM. TWICE IN A ROW. EAT IT.");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        Message("A side note about Labeler machines: you can click the big button on them to change the label which is applied, in order to meet demand shown in the top left corner!");
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(KeyDown);
        PostNotification("TutorialFinished");
        Message("HERE'S THE BIG KICKER: WHILE PLACING DOWN THE ROBOT, IN ORDER TO ASSIGN THE JOB, YOU MUST CLICK A STATION AT LEAST ONCE, AND IT WILL BE AUTOMATICALLY ASSIGNED TO THE BOT! Sorry for the caps, this one is important for you to read. Honestly, the most important. After assigning the job, finish pathing the bot and they're all set to do your work! Go ahead and try it now.");
        yield return new WaitForSeconds(1);
        ready = true;
    }
}
