using _SGUI_;
using _UTIL_;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ZOA_
{
    public abstract partial class ShellView : MonoBehaviour, SguiContextClick.ILeftClickable
    {
        public ShellField stdout_field, stdin_field;
        public TextMeshProUGUI tmp_progress;
        public ScrollRect scrollview;
        public RectTransform content_rT;
        public Scrollbar scrollbar;

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
            stdout_field = transform.Find("scrollview/viewport/content/std_out").GetComponent<ShellField>();
            stdin_field = stdout_field.transform.Find("std_in").GetComponent<ShellField>();

            tmp_progress = transform.Find("progress/text").GetComponent<TextMeshProUGUI>();

            scrollview = transform.Find("scrollview").GetComponent<ScrollRect>();

            content_rT = (RectTransform)transform.Find("scrollview/viewport/content");

            scrollbar = transform.Find("scrollview/scrollbar").GetComponent<Scrollbar>();

            shell?.Dispose();
            shell = null;
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Start()
        {
            stdout_field.rT.anchoredPosition = new Vector2(0, -offset_top_h);
            stdin_field.onValidateInput += OnValidateStdin_char;
            stdin_field.onValueChanged += OnStdinChanged;
            stdin_field.onSelect.AddListener(OnSelectStdin);
            stdin_field.onDeselect.AddListener(OnDeselectStdin);

            shell.Init();

            shell.prefixe.AddListener(value =>
            {
                stdin_field.text = value.text;
                stdin_field.lint.text = value.lint;
                stdin_field.caretPosition = value.text.Length;
            });

            shell.on_output += AddLine;

            shell.status.AddListener(value =>
            {
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

        public virtual void OnSguiContextClick(SguiContextClick_List context_list)
        {
            context_list.AddButton();
            context_list.AddButton();
            context_list.AddButton();
            context_list.AddButton();

            var button = context_list.AddButton();
            button.trad.SetTrad("1");
            button.SetupSublist(sublist =>
            {
                sublist.AddButton();
                sublist.AddButton();
                
                var sbutton = sublist.AddButton();
                sbutton.trad.SetTrad("2");

                sbutton.SetupSublist(ssublist =>
                {
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                    ssublist.AddButton();
                });

                sublist.AddButton();
            });

            context_list.AddButton();
            context_list.AddButton();
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            shell?.Dispose();
            shell = null;
        }
    }
}