using _ARK_;
using _SGUI_;
using _UTIL_;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ZOA_
{
    public partial class ShellView : MonoBehaviour
    {
        public SguiTerminal terminal;
        public ShellField stdout_field, stdin_field;
        public TextMeshProUGUI tmp_progress;
        public ScrollRect scrollview;
        public RectTransform content_rT;
        public Scrollbar scrollbar;

        public bool character_wrap;
        [SerializeField] float stdin_h, stdout_h;
        [SerializeField] bool flag_history;

        protected float
            offset_top_h = 2,
            offset_bottom_h = 5;

        public LintTheme lint_theme = LintTheme.theme_dark;
        public Shell shell;

        LintedString GetShellPrefixe() => shell.status._value switch
        {
            CMD_STATUS.BLOCKED => LintedString.EMPTY,
            _ => shell.prefixe._value,
        };

        //----------------------------------------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            InitShellHistory();
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
            character_wrap = true;

            terminal = GetComponentInParent<SguiTerminal>(true);

            stdout_field = transform.Find("scrollview/viewport/content/std_out").GetComponent<ShellField>();
            stdin_field = stdout_field.transform.Find("std_in").GetComponent<ShellField>();

            tmp_progress = transform.Find("progress/text").GetComponent<TextMeshProUGUI>();

            scrollview = transform.Find("scrollview").GetComponent<ScrollRect>();

            content_rT = (RectTransform)transform.Find("scrollview/viewport/content");

            scrollbar = transform.Find("scrollview/scrollbar").GetComponent<Scrollbar>();

            shell?.Dispose();
            shell = null;

            stdin_field.onSelect.AddListener(OnSelectStdin);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnEnable()
        {
            IMGUI_global.instance.clipboard_users.AddElement(OnClipboardOperation);
            IMGUI_global.instance.inputs_users.AddElement(OnImguiInputs);
        }

        protected virtual void OnDisable()
        {
            IMGUI_global.instance.clipboard_users.RemoveElement(OnClipboardOperation);
            IMGUI_global.instance.inputs_users.RemoveElement(OnImguiInputs);
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
            stdout_field.rT.anchoredPosition = new Vector2(0, -offset_top_h);
            stdin_field.onValidateInput += OnValidateStdin_char;
            stdin_field.onValueChanged.AddListener(OnStdinChanged);

            shell.Init();

            shell.prefixe.AddListener(value => CheckPrefixe());

            shell.on_output += AddLine;

            shell.status.AddListener(value =>
            {
                if (terminal != null)
                    if (value == CMD_STATUS.WAIT_FOR_STDIN)
                        terminal.trad_title.SetTrad(shell.GetType().Name);
                    else
                        terminal.trad_title.SetTrad($"{shell.GetType().Name}:{value}");

                switch (value)
                {
                    case CMD_STATUS.WAIT_FOR_STDIN:
                        ResetStdin();
                        break;

                    case CMD_STATUS.BLOCKED:
                        ResetStdin();
                        break;

                    case CMD_STATUS.NETWORKING:
                        break;
                }
            });

            ResetStdin();
            RefreshStdout();
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            shell?.Dispose();
            shell = null;
        }
    }
}