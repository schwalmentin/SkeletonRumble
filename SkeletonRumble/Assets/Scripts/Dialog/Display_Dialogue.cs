using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Display_Dialogue : MonoBehaviour
{
    public Dialogue testingDialogue;
    private Construct_Dialogue DialogueConstructor;
    [HideInInspector] public Dialogue currentDialogue;
    private GameManager gameManager;

    [Header("Do After Dialogue")]
    public int sceneToLoad;

    #region General Variables
    [Header("General")]
    public GameObject textCharacter;
    public GameObject textBox;
    [Range(0, 10)] public int maxTextBoxesAtATime;
    public float standartTimeBetweenAppearingCharacters = 0.1f;
    private float timeBetweenAppearingCharacters;
    public float fastForwardCharacterAppearingMultiplikator = 3f;
    private bool fastForwardCharacterAppearing;
    private float currentTimeBetweenAppearingCharacters = 0;
    [HideInInspector] public bool answer = false;
    private bool waitingForAnswer = false;

    //Object Pooling
    public int characterPoolSize = 1000;
    private Queue<GameObject> characterPool;
    private List<GameObject> currentCharacters = new List<GameObject>();
    private List<GameObject> allChararctersOnScreen = new List<GameObject>();
    private List<GameObject> currentWord = new List<GameObject>();

    public int textBoxPoolSize = 10;
    private Queue<GameObject> textBoxPool;
    private GameObject currentTextBox;
    private List<GameObject> allActiveTextBoxes = new List<GameObject>();

    //Commands
    private bool command;
    private List<char> currentCommand = new List<char>();
    private bool commandValue;
    private List<char> currentCommandValue = new List<char>();
    private Dictionary<string, string> activeCommands = new Dictionary<string, string>();

    //Manage Dialogue
    private int dialogueSegmentIndex = 0;
    private bool dialogueSegmentCompleted = true;
    private bool newTextBoxes;
    public int answerButtonIndex = 0;
    #endregion

    #region Positioning Variables
    [Header("Positioning Options")]
    public float yOffsetPerLine;
    public float charStartPositionX = -500;
    public float answerCharStartPositionX = -500;
    public float textBoxBorderRight = 500;
    private Vector2 nextCharPosition;
    public float spaceBetweenCharacters = 5;
    public float spaceBetweenLinesOfText = -50;
    private string currentInputDevice;
    private bool firstDevice;

    [System.Serializable]
    public struct TextBoxPositioning
    {
        public Vector2 TextBoxPivotPosition;
        public Vector2 TextBoxImagePositionRelativeToPivot;
        public Vector2 AnswerButtonPositionRelativeToPivot;

        public float pivotStartAngle;
        public float pivotEndAngle;
        public float rotationSpeed;

        public Sprite Keyboard_AnswerButton;
        public Sprite Neutral_AnswerButton;
        public Sprite PlayStation_AnswerButton;
        public Sprite Switch_AnswerButton;
        public Sprite XBox_AnswerButton;
    }
    public TextBoxPositioning[] textBoxPositionings;
    public TextBoxPositioning[] answerTextBoxPositionings;


    [System.Serializable]
    public struct TextBoxSize
    {
        public Sprite[] textBoxesAtThisSize;

        [Range(0, 10)] public float maxFittingText;

        [Space]
        public bool drawGizmos;
    }
    public TextBoxSize[] textBoxSizes;
    public TextBoxSize[] answerTextBoxSizes;
    private float ySizeOfText;
    private float xSizeOfText;

    [System.Serializable]
    public struct characterWidth
    {
        [Header("ABC")]
        [Range(0, 100)] public int A;
        [Range(0, 100)] public int B;
        [Range(0, 100)] public int C;
        [Range(0, 100)] public int D;
        [Range(0, 100)] public int E;
        [Range(0, 100)] public int F;
        [Range(0, 100)] public int G;
        [Range(0, 100)] public int H;
        [Range(0, 100)] public int I;
        [Range(0, 100)] public int J;
        [Range(0, 100)] public int K;
        [Range(0, 100)] public int L;
        [Range(0, 100)] public int M;
        [Range(0, 100)] public int N;
        [Range(0, 100)] public int O;
        [Range(0, 100)] public int P;
        [Range(0, 100)] public int Q;
        [Range(0, 100)] public int R;
        [Range(0, 100)] public int S;
        [Range(0, 100)] public int T;
        [Range(0, 100)] public int U;
        [Range(0, 100)] public int V;
        [Range(0, 100)] public int W;
        [Range(0, 100)] public int X;
        [Range(0, 100)] public int Y;
        [Range(0, 100)] public int Z;

        [Header("abc")]
        [Range(0, 100)] public int a;
        [Range(0, 100)] public int b;
        [Range(0, 100)] public int c;
        [Range(0, 100)] public int d;
        [Range(0, 100)] public int e;
        [Range(0, 100)] public int f;
        [Range(0, 100)] public int g;
        [Range(0, 100)] public int h;
        [Range(0, 100)] public int i;
        [Range(0, 100)] public int j;
        [Range(0, 100)] public int k;
        [Range(0, 100)] public int l;
        [Range(0, 100)] public int m;
        [Range(0, 100)] public int n;
        [Range(0, 100)] public int o;
        [Range(0, 100)] public int p;
        [Range(0, 100)] public int q;
        [Range(0, 100)] public int r;
        [Range(0, 100)] public int s;
        [Range(0, 100)] public int t;
        [Range(0, 100)] public int u;
        [Range(0, 100)] public int v;
        [Range(0, 100)] public int w;
        [Range(0, 100)] public int x;
        [Range(0, 100)] public int y;
        [Range(0, 100)] public int z;

        [Header("Numbers")]
        [Range(0, 100)] public int one;
        [Range(0, 100)] public int two;
        [Range(0, 100)] public int three;
        [Range(0, 100)] public int four;
        [Range(0, 100)] public int five;
        [Range(0, 100)] public int six;
        [Range(0, 100)] public int seven;
        [Range(0, 100)] public int eight;
        [Range(0, 100)] public int nine;
        [Range(0, 100)] public int zero;

        [Header("Special")]
        [Range(0, 100)] public int space;             // 
        [Range(0, 100)] public int dot;               //.
        [Range(0, 100)] public int comma;             //,
        [Range(0, 100)] public int questionMark;      //?
        [Range(0, 100)] public int exclamationMark;   //!
        [Range(0, 100)] public int quotationMark;     //"
        [Range(0, 100)] public int apostrophe;        //'
        [Range(0, 100)] public int dash;              //-
    }
    public characterWidth charactersWidthAtAFontSizeOf50;
    #endregion

    #region Effects Variables
    [Header("Effects")]
    public int standardFontSize = 50;
    private int currentFontSize;
    private float fontSizeWidthMultiplikator = 1;

    [Space]
    public Color standardFontColor = Color.black;
    private Color currentFontColor;
    public Color[] StandardizedColors;

    public enum OutlineSettings { hasNoOutline, hasOutlineWithAutomatedColor, hasFixedOutlineColor };
    [Space]
    public OutlineSettings outlineSettings;
    [Range(0, 5)] public float outlineWidth = 0.3f;
    [Range(-1, 1)] public float outlineColorBrightnessOffset = -0.1f;
    public Color outlineColorFixed = Color.black;

    [Space]
    public bool waveFromLeftToRight = true;
    public float waveHeight = 10;
    public float waveSpeed = 3;
    public float waveOffsetPerCharacter = 0.5f;
    private float currentWaveOffsetPerCharacter;
    private bool waveing;

    [Space]
    public float defaultShakingStrength = 3;

    [Space]
    public Color[] rainbowColors;
    private int rainbowColorIndex;
    #endregion

    void Start()
    {
        //Initalising
        this.InitializeTextBoxPool();
        this.InitializeCharacterPool();

        this.nextCharPosition.x = this.charStartPositionX;
        this.nextCharPosition.y = 0;

        this.DialogueConstructor = FindObjectOfType<Construct_Dialogue>();

        //this.currentDialogue = this.DialogueConstructor.ConstructDialogue(true);

        //this.DisplayDialogue(this.currentDialogue);

        this.gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        //Updates every Frame
        this.UpdateAppearingCharacters();
    }


    //Inputs
    public void ShowDialogueButton(int answer)
    {
        if(this.dialogueSegmentCompleted && this.waitingForAnswer)
        {
            this.EvaluateAnswer(answer);
        }
    }
    public void ShowDialogue(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        this.DisplayDialogue(this.currentDialogue);
    }
    public void RecieveAnswer1(InputAction.CallbackContext context)
    {
        if (context.performed && this.dialogueSegmentCompleted && this.waitingForAnswer)
        {
            this.EvaluateAnswer(0);
        }
    }
    public void RecieveAnswer2(InputAction.CallbackContext context)
    {
        if (context.performed && this.dialogueSegmentCompleted && this.waitingForAnswer)
        {
            this.EvaluateAnswer(1);
        }
    }
    public void RecieveAnswer3(InputAction.CallbackContext context)
    {
        if (context.performed && this.dialogueSegmentCompleted && this.waitingForAnswer)
        {
            this.EvaluateAnswer(2);
        }
    }
    public void RecieveAnswer4(InputAction.CallbackContext context)
    {
        if (context.performed && this.dialogueSegmentCompleted && this.waitingForAnswer)
        {
            this.EvaluateAnswer(3);
        }
    }
    private void EvaluateAnswer(int index)
    {
        if (this.currentDialogue.answers[0] != this.DialogueConstructor.commandToStartCombat)
        {
            this.ApplyEmotionalChangesFromAnAnswer(index);
            this.currentDialogue = this.DialogueConstructor.ConstructDialogue(false);

            this.dialogueSegmentIndex = 0;
            this.ResetCharactersAndTextboxes();
            this.answer = false;
            this.waitingForAnswer = false;
            this.DisplayDialogue(this.currentDialogue);

            this.gameManager.UpdateEnemyEmotionAnimation();
            this.gameManager.PlayEmotionAction();
        }
    }
    private void ApplyEmotionalChangesFromAnAnswer(int index)
    {
        this.DialogueConstructor.debugFileDisplay.text += this.currentDialogue.emotionalChange[index].dialogueIndex;

        if (this.currentDialogue.emotionalChange[index].emotion == Dialogue.EmotionalChange.Emotion.Anger)
        { EmotionManager.Anger += this.currentDialogue.emotionalChange[index].value; }

        if (this.currentDialogue.emotionalChange[index].emotion == Dialogue.EmotionalChange.Emotion.Arrogance)
        { EmotionManager.Arrogance += this.currentDialogue.emotionalChange[index].value; }

        if (this.currentDialogue.emotionalChange[index].emotion == Dialogue.EmotionalChange.Emotion.Elegance)
        { EmotionManager.Elegance += this.currentDialogue.emotionalChange[index].value; }

        if (this.currentDialogue.emotionalChange[index].emotion == Dialogue.EmotionalChange.Emotion.Fear)
        { EmotionManager.Fear += this.currentDialogue.emotionalChange[index].value; }

        this.answerButtonIndex = 0;
    }

    public void ChangeButtonPrompts(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        InputDevice device = context.control.device;
        if (device.displayName == this.currentInputDevice) { return; }

        if (this.currentInputDevice == "") { this.firstDevice = true; }

        this.currentInputDevice = device.displayName;
        print(this.currentInputDevice);

        //if (this.answer || this.firstDevice)
        //{
        //    this.firstDevice = false;

        //    if (device.displayName == "Keyboard" || device.displayName == "Mouse")
        //    {
        //        for (int i = 0; i < maxTextBoxesAtATime; i++)
        //        {
        //            Transform[] TextBoxChildren = allActiveTextBoxes[i].GetComponentsInChildren<Transform>(true);
        //            TextBoxChildren[3].GetComponent<Image>().sprite = answerTextBoxPositionings[i].Keyboard_AnswerButton;
        //            TextBoxChildren[3].GetComponent<Image>().SetNativeSize();
        //        }
        //        return;
        //    }

        //    if (device.displayName == "Xbox Controller")
        //    {
        //        for (int i = 0; i < maxTextBoxesAtATime; i++)
        //        {
        //            Transform[] TextBoxChildren = allActiveTextBoxes[i].GetComponentsInChildren<Transform>(true);
        //            TextBoxChildren[3].GetComponent<Image>().sprite = answerTextBoxPositionings[i].XBox_AnswerButton;
        //            TextBoxChildren[3].GetComponent<Image>().SetNativeSize();
        //        }
        //        return;
        //    }

        //    if (device.displayName == "PlayStation Controller" || device.displayName == "Wireless Controller")
        //    {
        //        for (int i = 0; i < maxTextBoxesAtATime; i++)
        //        {
        //            Transform[] TextBoxChildren = allActiveTextBoxes[i].GetComponentsInChildren<Transform>(true);
        //            TextBoxChildren[3].GetComponent<Image>().sprite = answerTextBoxPositionings[i].PlayStation_AnswerButton;
        //            TextBoxChildren[3].GetComponent<Image>().SetNativeSize();
        //        }
        //        return;
        //    }

        //    if (device.displayName == "Switch Controller" || device.displayName == "Pro Controller")
        //    {
        //        for (int i = 0; i < maxTextBoxesAtATime; i++)
        //        {
        //            Transform[] TextBoxChildren = allActiveTextBoxes[i].GetComponentsInChildren<Transform>(true);
        //            TextBoxChildren[3].GetComponent<Image>().sprite = answerTextBoxPositionings[i].Switch_AnswerButton;
        //            TextBoxChildren[3].GetComponent<Image>().SetNativeSize();
        //        }
        //        return;
        //    }

        //    //Wenn das Device nicht ermittelt werden kann, dann zeig ein neutrales Icon an.
        //    for (int i = 0; i < maxTextBoxesAtATime; i++)
        //    {
        //        Transform[] TextBoxChildren = allActiveTextBoxes[i].GetComponentsInChildren<Transform>(true);
        //        TextBoxChildren[3].GetComponent<Image>().sprite = answerTextBoxPositionings[i].Neutral_AnswerButton;
        //        TextBoxChildren[3].GetComponent<Image>().SetNativeSize();
        //    }
        //}
    }


    //Displaying The Dialogue
    public void DisplayDialogue(Dialogue inputDialogue)
    {
        if (this.dialogueSegmentCompleted && !this.waitingForAnswer)
        {
            //Switch to displaying the Answers when all dialogue has been displayed
            if (this.dialogueSegmentIndex == inputDialogue.dialogueSegments.Length)
            {
                this.dialogueSegmentIndex = 0;

                this.ResetCharactersAndTextboxes();

                this.answer = true;
                this.waitingForAnswer = true;
            }

            if (inputDialogue.answers[0] == this.DialogueConstructor.commandToStartCombat && this.answer)     // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!
            {                                                                                       // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!
                this.answer = false;
                this.waitingForAnswer = false;                                                      // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!
                this.gameManager.OnEndDialog();                                                     // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!
                return;                                                                             // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!
            }                                                                                       // !!!!!!!!!!!!! START BATTLE HERE !!!!!!!!!!!!!

            if (this.allActiveTextBoxes.Count == this.maxTextBoxesAtATime || this.newTextBoxes) { this.ResetCharactersAndTextboxes(); this.newTextBoxes = false; } //Reset the Text and the Textboxes when the maximum has already been spawned

            int x;
            if (!this.answer) { x = 1; }
            else { x = inputDialogue.answers.Length; if (x > this.maxTextBoxesAtATime) { x = this.maxTextBoxesAtATime; } }//Just For Now

            for (int doneEverything = 0; doneEverything < x; doneEverything++)
            {
                this.currentTextBox = this.textBoxPool.Dequeue();
                this.currentTextBox.SetActive(true);
                this.allActiveTextBoxes.Add(this.currentTextBox);
                this.ySizeOfText = Mathf.Abs(this.spaceBetweenLinesOfText);

                Transform[] currentTextBoxChildren = this.currentTextBox.GetComponentsInChildren<Transform>(true);
                if (!this.answer)
                {
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().startAngle = this.textBoxPositionings[this.allActiveTextBoxes.Count - 1].pivotStartAngle;
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().endAngle = this.textBoxPositionings[this.allActiveTextBoxes.Count - 1].pivotEndAngle;
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().rotationSpeed = this.textBoxPositionings[this.allActiveTextBoxes.Count - 1].rotationSpeed;

                    currentTextBoxChildren[1].localPosition = this.textBoxPositionings[this.allActiveTextBoxes.Count - 1].TextBoxPivotPosition;                //Set the Position of the Pivot from the current TextBox
                    currentTextBoxChildren[2].localPosition = this.textBoxPositionings[this.allActiveTextBoxes.Count - 1].TextBoxImagePositionRelativeToPivot; //Set the Position of the Image relative to the Position of the Pivot from the current TextBox
                }
                else
                {
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().startAngle = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].pivotStartAngle;
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().endAngle = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].pivotEndAngle;
                    currentTextBoxChildren[0].GetComponent<DialogueBox>().rotationSpeed = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].rotationSpeed;

                    currentTextBoxChildren[1].localPosition = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].TextBoxPivotPosition;                //Set the Position of the Pivot from the current TextBox
                    currentTextBoxChildren[2].localPosition = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].TextBoxImagePositionRelativeToPivot; //Set the Position of the Image relative to the Position of the Pivot from the current TextBox
                    currentTextBoxChildren[2].GetComponent<Button>().enabled = true;
                    currentTextBoxChildren[3].gameObject.SetActive(true);
                    currentTextBoxChildren[3].localPosition = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].AnswerButtonPositionRelativeToPivot; //Set the Position of the Button-Image relative to the Position of the Pivot from the current TextBox
                    currentTextBoxChildren[3].GetComponent<Image>().sprite = this.answerTextBoxPositionings[this.allActiveTextBoxes.Count - 1].Keyboard_AnswerButton;
                    currentTextBoxChildren[3].GetComponent<Image>().SetNativeSize();
                }


                //Reset Certain Values for the next TextBox
                this.dialogueSegmentCompleted = false;
                this.fastForwardCharacterAppearing = false;

                if (this.currentCharacters != null)
                {
                    this.currentWaveOffsetPerCharacter = 0;
                    this.currentTimeBetweenAppearingCharacters = 0;
                    this.activeCommands.Clear();
                    this.currentCharacters.Clear();
                }

                if (!this.answer) { this.nextCharPosition.x = this.charStartPositionX; }
                else { this.nextCharPosition.x = this.answerCharStartPositionX; }
                this.nextCharPosition.y = 0;

                //Convert the inputText into individual Characters and loop though them
                char[] characters;
                if (!this.answer)
                {
                    characters = inputDialogue.dialogueSegments[this.dialogueSegmentIndex].ToCharArray();
                }
                else
                {
                    characters = inputDialogue.answers[this.dialogueSegmentIndex].ToCharArray();
                }

                int index_TextCharacters = 0;

                foreach (char character in characters)
                {
                    //Checking for commands and stop them from beeing written out

                    if (character == '<')
                    {
                        this.command = true;
                        this.currentCommand.Clear();
                        this.currentCommandValue.Clear();

                        continue;
                    }   //Start command
                    if (character == '>')
                    {
                        this.command = false;
                        this.commandValue = false;
                        if (this.activeCommands.Keys.Count > 0)
                        {
                            bool commandEditedOrRemoved = false;
                            foreach (string command in this.activeCommands.Keys)
                            {
                                if (command == string.Concat(this.currentCommand))
                                {
                                    if (this.activeCommands[command].Contains(string.Concat(this.currentCommandValue)) || this.currentCommandValue.Count == 0)
                                    {

                                        this.activeCommands.Remove(command);
                                    }
                                    else
                                    {
                                        this.activeCommands[command] = string.Concat(this.currentCommandValue);
                                    }

                                    commandEditedOrRemoved = true;
                                    break;
                                }
                            }

                            if (!commandEditedOrRemoved)
                            {
                                this.activeCommands.Add(string.Concat(this.currentCommand), string.Concat(this.currentCommandValue));
                            }
                        }
                        else
                        {
                            this.activeCommands.Add(string.Concat(this.currentCommand), string.Concat(this.currentCommandValue));
                        }
                        continue;
                    }   //End command and activate/deactivate the command by adding/removing the entry of it in the activeCommands dictionary
                    if (character == ',' && this.command || character == ',' && this.commandValue)
                    {
                        if (this.activeCommands.Keys.Count > 0)
                        {
                            bool commandEditedOrRemoved = false;
                            foreach (string command in this.activeCommands.Keys)
                            {
                                if (command == string.Concat(this.currentCommand))
                                {
                                    if (this.activeCommands[command].Contains(string.Concat(this.currentCommandValue)))
                                    {
                                        this.activeCommands.Remove(command);
                                    }
                                    else
                                    {
                                        this.activeCommands[command] = string.Concat(this.currentCommandValue);
                                    }

                                    commandEditedOrRemoved = true;
                                    break;
                                }
                            }

                            if (!commandEditedOrRemoved)
                            {
                                this.activeCommands.Add(string.Concat(this.currentCommand), string.Concat(this.currentCommandValue));
                            }
                        }
                        else
                        {
                            this.activeCommands.Add(string.Concat(this.currentCommand), string.Concat(this.currentCommandValue));
                        }

                        this.command = true;
                        this.commandValue = false;
                        this.currentCommand.Clear();
                        this.currentCommandValue.Clear();

                        continue;
                    }   //End command and start the next command in order to chain together multiple commands
                    if (this.command)
                    {
                        if (character == '=')
                        {
                            this.command = false;
                            this.commandValue = true;

                            continue;
                        }

                        this.currentCommand.Add(character);

                        continue;
                    }            //Save command and switch to the value if the command has a = in it
                    if (this.commandValue)
                    {
                        this.currentCommandValue.Add(character);

                        continue;
                    }       //Save the value of the command if it has one

                    if (character == '|')
                    {
                        foreach (GameObject txtChar in this.currentCharacters)
                        {
                            txtChar.GetComponent<RectTransform>().position = new Vector2(txtChar.transform.position.x, txtChar.transform.position.y + Mathf.Abs(this.spaceBetweenLinesOfText) / 2);
                        }

                        if (!this.answer) { this.nextCharPosition.x = this.charStartPositionX; }
                        else { this.nextCharPosition.x = this.answerCharStartPositionX; }
                        this.nextCharPosition.y = (this.nextCharPosition.y + this.spaceBetweenLinesOfText / 2);
                        this.ySizeOfText += Mathf.Abs(this.spaceBetweenLinesOfText);

                        continue;
                    }  //New Line in the same TextBox


                    //Instantiate a new Character and apply the current Format (for example: Bold)
                    GameObject textChar = this.characterPool.Dequeue();
                    this.currentCharacters.Add(textChar);
                    this.allChararctersOnScreen.Add(textChar);

                    textChar.GetComponent<RectTransform>().SetParent(currentTextBoxChildren[2]);
                    textChar.GetComponent<RectTransform>().localPosition = new Vector2(this.nextCharPosition.x, this.nextCharPosition.y);
                    textChar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);

                    if (character == ' ')
                    {
                        this.currentWord.Clear();
                    }   //Start new word/character-chain
                    else { this.currentWord.Add(textChar); }

                    this.timeBetweenAppearingCharacters = this.standartTimeBetweenAppearingCharacters;
                    textChar.GetComponentInChildren<TMP_Text>().faceColor = this.standardFontColor;
                    this.currentFontSize = this.standardFontSize;
                    textChar.GetComponent<Character>().waveFromLeftToRight = this.waveFromLeftToRight;

                    this.ExecuteActiveCommands(textChar, inputDialogue);

                    this.currentTimeBetweenAppearingCharacters += this.timeBetweenAppearingCharacters;
                    textChar.GetComponent<Character>().appearTimer = this.currentTimeBetweenAppearingCharacters;

                    textChar.GetComponentInChildren<TMP_Text>().fontSize = this.currentFontSize;
                    this.fontSizeWidthMultiplikator = (float)this.currentFontSize / 50; //Divided by 50 because the "charactersWidthAtAFontSizeOf50" uses 50 as it's default FontSize

                    //Write the Character into the new TextField
                    textChar.GetComponentInChildren<TMP_Text>().text = character.ToString();

                    index_TextCharacters++;

                    //Apply the custom width for the next character
                    this.ApplyWidthBetweenCharacters(character);

                    //Start a new Line and position the words correctly when the text exceeds the rigth textBox border
                    if (textChar.transform.localPosition.x > this.textBoxBorderRight && character != ' ')
                    {
                        foreach (GameObject txtChar in this.currentCharacters)
                        {
                            txtChar.GetComponent<RectTransform>().position = new Vector2(txtChar.transform.position.x, txtChar.transform.position.y + Mathf.Abs(this.spaceBetweenLinesOfText) / 2);
                        }

                        this.nextCharPosition.y = this.nextCharPosition.y + this.spaceBetweenLinesOfText / 2;
                        if (!this.answer) { this.nextCharPosition.x = this.charStartPositionX; }
                        else { this.nextCharPosition.x = this.answerCharStartPositionX; }

                        foreach (GameObject wordCharacters in this.currentWord)
                        {
                            wordCharacters.GetComponent<RectTransform>().localPosition = new Vector2(this.nextCharPosition.x, this.nextCharPosition.y);

                            char[] characterInTextfield = wordCharacters.GetComponentInChildren<TMP_Text>().text.ToCharArray();
                            this.ApplyWidthBetweenCharacters(characterInTextfield[0]);
                        }

                        this.ySizeOfText += Mathf.Abs(this.spaceBetweenLinesOfText);
                    }

                }   //Create and Manage ALL Characters (including commands)

                //Change the look and size of the Textbox depending on how much text is in it
                float smallestFittingTextBoxSize = float.MaxValue;
                int fittingTextBoxIndex = 0;

                if (!this.answer)
                {
                    for (int i = 0; i < this.textBoxSizes.Length; i++)
                    {
                        if (this.textBoxSizes[i].maxFittingText >= this.ySizeOfText && this.textBoxSizes[i].maxFittingText < smallestFittingTextBoxSize)
                        {
                            smallestFittingTextBoxSize = this.textBoxSizes[i].maxFittingText;
                            fittingTextBoxIndex = i;
                        }
                    }

                    this.currentTextBox.GetComponentInChildren<Image>().sprite = this.textBoxSizes[fittingTextBoxIndex].textBoxesAtThisSize[Random.Range(0, this.textBoxSizes[fittingTextBoxIndex].textBoxesAtThisSize.Length)];
                    this.currentTextBox.GetComponentInChildren<Image>().SetNativeSize();
                }
                else
                {
                    for (int i = 0; i < this.answerTextBoxSizes.Length; i++)
                    {
                        if (this.answerTextBoxSizes[i].maxFittingText >= this.ySizeOfText && this.answerTextBoxSizes[i].maxFittingText < smallestFittingTextBoxSize)
                        {
                            smallestFittingTextBoxSize = this.answerTextBoxSizes[i].maxFittingText;
                            fittingTextBoxIndex = i;
                        }
                    }

                    this.currentTextBox.GetComponentInChildren<Image>().sprite = this.answerTextBoxSizes[fittingTextBoxIndex].textBoxesAtThisSize[Random.Range(0, this.answerTextBoxSizes[fittingTextBoxIndex].textBoxesAtThisSize.Length)];
                    this.currentTextBox.GetComponentInChildren<Image>().SetNativeSize();
                }

                this.dialogueSegmentIndex++;
            }
        }
        else
        {
            if (!this.fastForwardCharacterAppearing && this.allChararctersOnScreen.Count != 0)
            {
                foreach (GameObject textChar in this.allChararctersOnScreen)
                {
                    textChar.GetComponent<Character>().appearTimer /= this.fastForwardCharacterAppearingMultiplikator;
                }
                this.fastForwardCharacterAppearing = true;
            }
        }
    }

    private void AddYOffset()
    {
        foreach (var item in this.allChararctersOnScreen)
        {
            item.transform.localPosition = new Vector2(item.transform.localPosition.x, item.transform.localPosition.y - this.yOffsetPerLine);
        }
    }

    private void ExecuteActiveCommands(GameObject textChar, Dialogue inputDialogue)
    {
        CultureInfo parseInEnglisch = CultureInfo.CreateSpecificCulture("en-US");   //Make sure Parsing always uses english and not the operating-system language (important for differences like , and .)

        //Area Effects (affect an Area with a desegnated start and end-point)
        if (this.activeCommands.ContainsKey("RE"))
        {

            this.activeCommands.Clear();

        }   //Reset Effects
        if (this.activeCommands.ContainsKey("B"))
        {

            textChar.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
            textChar.GetComponentInChildren<TMP_Text>().fontSize += 7;
        }    //Bold
        if (this.activeCommands.ContainsKey("W"))
        {

            textChar.GetComponent<Character>().waveing = true;
            textChar.GetComponent<Character>().waveOffset = this.currentWaveOffsetPerCharacter;
            this.currentWaveOffsetPerCharacter += this.waveOffsetPerCharacter;

        }    //Waving                
        if (this.activeCommands.ContainsKey("S"))
        {
            textChar.GetComponent<Character>().shaking = true;
            if (this.activeCommands["S"] != "")
            {
                if (float.TryParse(this.activeCommands["S"], NumberStyles.Float, parseInEnglisch, out float value))
                {
                    textChar.GetComponent<Character>().shakingStrength = value;
                }
                else { Debug.LogWarning("Error: The Command 'S' does not accept '" + this.activeCommands["S"].ToString() + "' as its value!"); }
            }
            else
            {
                textChar.GetComponent<Character>().shakingStrength = this.defaultShakingStrength;
            }

        }    //Shaking               
        if (this.activeCommands.ContainsKey("F"))
        {

        }    //Falling Letters       MISSING
        if (this.activeCommands.ContainsKey("R"))
        {

            textChar.GetComponentInChildren<TMP_Text>().faceColor = this.rainbowColors[this.rainbowColorIndex];

            switch (this.outlineSettings)
            {
                case OutlineSettings.hasNoOutline:
                    break;

                case OutlineSettings.hasOutlineWithAutomatedColor:
                    textChar.GetComponentInChildren<TMP_Text>().outlineWidth = this.outlineWidth;
                    textChar.GetComponentInChildren<TMP_Text>().outlineColor = this.ChangeColorBrightness(this.rainbowColors[this.rainbowColorIndex], this.outlineColorBrightnessOffset);
                    break;

                case OutlineSettings.hasFixedOutlineColor:
                    textChar.GetComponentInChildren<TMP_Text>().outlineWidth = this.outlineWidth;
                    textChar.GetComponentInChildren<TMP_Text>().outlineColor = this.outlineColorFixed;
                    break;
                default:
                    break;
            }

            if (this.rainbowColorIndex == this.rainbowColors.Length - 1)
            {
                this.rainbowColorIndex = 0;
            }
            else
            {
                this.rainbowColorIndex++;
            }

        }    //Rainbow
        if (this.activeCommands.ContainsKey("FS"))
        {

            if (int.TryParse(this.activeCommands["FS"], NumberStyles.Integer, parseInEnglisch, out int val))
            {
                this.currentFontSize = val;
            }
            else { Debug.LogWarning("Error: The Command 'FS' does not accept '" + this.activeCommands["FS"].ToString() + "' as its value!"); }

        }   //FontSize = 50
        if (this.activeCommands.ContainsKey("Co"))
        {
            bool isStandardized = false;
            Color color;
            for (int i = 0; i < this.StandardizedColors.Length; i++)
            {
                if (this.activeCommands["Co"] == "St" + (i + 1)) { textChar.GetComponentInChildren<TMP_Text>().faceColor = this.StandardizedColors[i]; isStandardized = true; break; }
            }
            
            if (ColorUtility.TryParseHtmlString(this.activeCommands["Co"], out color) && !isStandardized)
            {
                textChar.GetComponentInChildren<TMP_Text>().faceColor = color;

                switch (this.outlineSettings)
                {
                    case OutlineSettings.hasNoOutline:
                        break;

                    case OutlineSettings.hasOutlineWithAutomatedColor:
                        textChar.GetComponentInChildren<TMP_Text>().outlineWidth = this.outlineWidth;
                        textChar.GetComponentInChildren<TMP_Text>().outlineColor = this.ChangeColorBrightness(color, this.outlineColorBrightnessOffset);
                        break;

                    case OutlineSettings.hasFixedOutlineColor:
                        textChar.GetComponentInChildren<TMP_Text>().outlineWidth = this.outlineWidth;
                        textChar.GetComponentInChildren<TMP_Text>().outlineColor = this.outlineColorFixed;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!isStandardized)
                {
                    Debug.LogWarning("Error: The Command 'Co' does not accept '" + this.activeCommands["Co"].ToString() + "' as its value!");
                }
            }

        }   //Color = #ffffffff
        if (this.activeCommands.ContainsKey("TS"))
        {
            if (float.TryParse(this.activeCommands["TS"], NumberStyles.Float, parseInEnglisch, out float val))
            {
                this.timeBetweenAppearingCharacters = val;
            }
            else { Debug.LogWarning("Error: The Command 'TS' does not accept '" + this.activeCommands["TS"].ToString() + "' as its value!"); }

        }   //Typing Speed = 0.1f
        if (this.activeCommands.ContainsKey("TD"))
        {
            if (float.TryParse(this.activeCommands["TD"], NumberStyles.Float, parseInEnglisch, out float val))
            {
                this.currentTimeBetweenAppearingCharacters += val;
            }
            else { Debug.LogWarning("Error: The Command 'TD' does not accept '" + this.activeCommands["TD"].ToString() + "' as its value!"); }
            this.activeCommands.Remove("TD");

        }   //Typing Delay = 0.3f
        if (this.activeCommands.ContainsKey("WB"))
        {

        }   //Wall Break            MISSING
        if (this.activeCommands.ContainsKey("AS"))
        {

        }   //Appearing Shake       MISSING
        if (this.activeCommands.ContainsKey("N"))
        {
            this.newTextBoxes = true;
            this.activeCommands.Remove("NT");
        }    //New Textboxes         NOT WORKING     ==> because the bool gets set inside of the large 'character foreach' and the textboxes are outside of the foreach at the start of the script.


        if (this.activeCommands.Keys.Count > 0)
        {
            foreach (string command in this.activeCommands.Keys)
            {
                if (command != "RE" &&
                    command != "B" &&
                    command != "W" &&
                    command != "S" &&
                    command != "F" &&
                    command != "R" &&
                    command != "FS" &&
                    command != "Co" &&
                    command != "TS" &&
                    command != "TD" &&
                    command != "WB" &&
                    command != "AS" &&
                    command != "NT")
                {
                    Debug.LogWarning("Error: Invalid Command: '" + command + "'; It can be found in the dialogueSegment " + this.dialogueSegmentIndex);
                }
            }
        }      //Point out invalid commands   <- Has to be manually added when creating a new effect!

    }

    private void ResetCharactersAndTextboxes()
    {
        foreach (GameObject textChar in this.allChararctersOnScreen)
        {
            textChar.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Normal;
            textChar.GetComponentInChildren<TMP_Text>().faceColor = this.standardFontColor;
            textChar.GetComponentInChildren<TMP_Text>().outlineWidth = 0;

            textChar.GetComponent<Character>().waveing = false;
            textChar.GetComponent<Character>().shaking = false;

            textChar.transform.SetParent(this.transform);
            textChar.transform.rotation = Quaternion.Euler(0, 0, 0);
            textChar.SetActive(false);
            this.characterPool.Enqueue(textChar);
        }

        foreach (GameObject activeTextBox in this.allActiveTextBoxes)
        {
            Transform[] currentTextBoxChildren = activeTextBox.GetComponentsInChildren<Transform>(true);
            currentTextBoxChildren[2].GetComponent<Button>().enabled = false;
            currentTextBoxChildren[3].gameObject.SetActive(false);
            activeTextBox.SetActive(false);
            this.textBoxPool.Enqueue(activeTextBox);
        }

        this.allChararctersOnScreen.Clear();
        this.allActiveTextBoxes.Clear();
    }

    private void UpdateAppearingCharacters()
    {
        int i = 0;
        if (this.allChararctersOnScreen.Count != 0)
        {

            foreach (GameObject currentCharacter in this.allChararctersOnScreen)
            {

                currentCharacter.GetComponent<Character>().appearTimer -= Time.deltaTime;
                if (currentCharacter.GetComponent<Character>().appearTimer < 0)
                {
                    currentCharacter.SetActive(true);

                    if (this.currentCharacters[this.currentCharacters.Count - 1].activeSelf == true)
                    {
                        i++;
                    }
                }
            }

            if (i == this.allChararctersOnScreen.Count)
            {
                this.dialogueSegmentCompleted = true;
            }
        }
    }    //Handles the updating of the typewriter effect and sets a bool true when all charcters have been written out

    private void InitializeCharacterPool()
    {
        this.characterPool = new Queue<GameObject>();

        for (int i = 0; i < this.characterPoolSize; i++)
        {
            GameObject newGO = Instantiate(this.textCharacter, this.transform, false);

            newGO.GetComponent<Character>().waveHeight = this.waveHeight;
            newGO.GetComponent<Character>().waveSpeed = this.waveSpeed;
            newGO.SetActive(false);

            this.characterPool.Enqueue(newGO);
        }
    }

    private void InitializeTextBoxPool()
    {
        this.textBoxPool = new Queue<GameObject>();

        for (int i = 0; i < this.textBoxPoolSize; i++)
        {
            GameObject newGO = Instantiate(this.textBox, this.transform, false);

            newGO.SetActive(false);

            this.textBoxPool.Enqueue(newGO);
        }
    }

    private Color ChangeColorBrightness(Color color, float correctionFactor)
    {
        float hue;
        float saturation;
        float brightness;
        Color.RGBToHSV(color, out hue, out saturation, out brightness);
        color = Color.HSVToRGB(hue, saturation, brightness + correctionFactor);

        return color;
    }

    private void ApplyWidthBetweenCharacters(char character)
    {
        if (character == 'A') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.A * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'B') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.B * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'C') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.C * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'D') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.D * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'E') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.E * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'F') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.F * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'G') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.G * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'H') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.H * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'I') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.I * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'J') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.J * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'K') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.K * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'L') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.L * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'M') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.M * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'N') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.N * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'O') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.O * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'P') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.P * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'Q') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.Q * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'R') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.R * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'S') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.S * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'T') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.T * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'U') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.U * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'V') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.V * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'W') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.W * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'X') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.X * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'Y') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.Y * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'Z') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.Z * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }

        if (character == 'a') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.a * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'b') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.b * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'c') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.c * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'd') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.d * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'e') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.e * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'f') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.f * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'g') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.g * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'h') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.h * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'i') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.i * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'j') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.j * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'k') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.k * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'l') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.l * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'm') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.m * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'n') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.n * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'o') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.o * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'p') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.p * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'q') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.q * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'r') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.r * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 's') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.s * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 't') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.t * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'u') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.u * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'v') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.v * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'w') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.w * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'x') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.x * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'y') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.y * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == 'z') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.z * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }

        if (character == '1') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.one * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '2') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.two * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '3') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.three * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '4') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.four * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '5') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.five * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '6') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.six * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '7') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.seven * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '8') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.eight * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '9') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.nine * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '0') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.zero * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }

        if (character == ' ') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.space * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '.') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.dot * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == ',') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.comma * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '?') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.questionMark * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '!') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.exclamationMark * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '"') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.quotationMark * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '\'') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.apostrophe * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
        if (character == '-') { this.nextCharPosition.x += this.charactersWidthAtAFontSizeOf50.dash * this.fontSizeWidthMultiplikator + this.spaceBetweenCharacters; }
    }   //Apply the custom width for the next character

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        for (int i = 0; i < this.textBoxSizes.Length; i++)
        {
            if (this.textBoxSizes[i].drawGizmos)
            {
                Gizmos.DrawLine(new Vector3(this.transform.position.x + (i), this.transform.position.y - this.textBoxSizes[i].maxFittingText / 2, this.transform.position.z), new Vector3(this.transform.position.x + (i), this.transform.position.y + this.textBoxSizes[i].maxFittingText / 2, this.transform.position.z));
            }
        }
    }
}

/*  To Do:
 *  -> Antworten
 */

/*  Next Steps:
 *  Use segments to determin Text Boxes
 *  Fix Positioning of Waving/Shaking Text -On hold
 *  Pick appropriately sized Text Box (Y Axis) -Works for now   ==> Doesn't consider Factors like Fontsize rigth now
 *  Pick appropriately sized Text Box (X Axis) -ignore for now
 *  Position Text correctly (Rechts/Linksbündig) -ignore for now
 *  Position TextBoxes Correctly
 *  Delete Text Boxes After Answering
 *  Add Answering
 */