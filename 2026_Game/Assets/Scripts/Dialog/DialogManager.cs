using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using System.Collections;


public class DialogManager : MonoBehaviour
{
    public static DialogManager instance { get; private set; }

    [Header("Dialog Reterences")]
    [SerializeField] private DialogDatabaseSO dialogDatabase;

    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image portraitImage;                       //캐릭터 초상화 이미지 UI 생성

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Button NextButton;

    [Header("UI References")]
    [SerializeField] private float typingSpeed = 0.0f;
    [SerializeField] private bool useTypewriterEffect = true;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private DialgSO currentDialog;


    private DialgSO curentDialog;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogDatabase != null)
        {
            dialogDatabase.Initailize();                //초기화
        }
        else
        {
            Debug.LogError("Dialog Datebase is not assiged to Dialog Manger");
        }
        if (NextButton != null)
        {
            NextButton.onClick.AddListener(NextDialog);      //버튼 리스너 등록
        }
        else
        {
            Debug.LogError("Next Button is Not assigend1");
        }

    }

    void Start()
    {
        //UI 초기화 후 대화 시작(id 1)
        CloseDialog();
        StartDialog(1);         //자동으로 첫 번째 대화 시작
    }


    void Update()
    {
        
    }

    //ID로 대화시작
    public void StartDialog(int dialogId)
    {
        DialgSO dialog = dialogDatabase.GetDialongById(dialogId);
        if (dialog != null)
        {
            StartDialog(dialog);
        }
        else
        {
            Debug.LogError($"Dialog with ID{dialogId} not found");
        }
    }

    public void StartDialog(DialgSO dialog)
    {
        if (dialog == null) return;

        curentDialog = dialog;
        ShowDialog();
        dialogPanel.SetActive(true);
    }

    public void ShowDialog()
    {

        Debug.Log(curentDialog.portraitPath);

        if (curentDialog == null) return;
        characterNameText.text = curentDialog.characterName;        //캐릭터 이름 설정

        if (useTypewriterEffect)
        {
            StartTypingEffect(curentDialog.text);
        }
        else
        {
            dialogText.text = curentDialog.text;        //대화 텍스트 생성
        }



        //초상화 설정(새로 추가된 부분)
        if (curentDialog.protrait != null)
        {
            portraitImage.sprite = curentDialog.protrait;
            portraitImage.gameObject.SetActive(true);
        }
        else if (!string.IsNullOrEmpty(curentDialog.portraitPath))
        {
            //Resources 폴더에서  이미지 로드 (Assets/Resources/~)
            Sprite portrait = Resources.Load<Sprite>(curentDialog.portraitPath);
            if (portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Portrait not found at path : {curentDialog.portraitPath}");
                portraitImage.gameObject.SetActive(false);
            }
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

    }

    public void CloseDialog()           //대화 종료
    {
        dialogPanel.SetActive(false);
        curentDialog = null;
        StopTypingEffect();
    }

    public void NextDialog()
    {
        if (isTyping)
        {
            StopTypingEffect();
            dialogText.text = curentDialog.text;
            isTyping = false;
            return;
        }



        if (curentDialog != null &&curentDialog.nextId>0) {
            DialgSO nextDialog = dialogDatabase.GetDialongById(curentDialog.nextId);
            if (nextDialog != null)
            {
                curentDialog = nextDialog;
                ShowDialog();
            }
            else
            {
                CloseDialog();
            }
        }
        else
        {
            CloseDialog();
        }
    }
    

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach(char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    //타이핑 효과 함수 시작
    private void StopTypingEffect()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
    }

    private void StartTypingEffect(string text)
    {
        isTyping = true;
        if(typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }




}
