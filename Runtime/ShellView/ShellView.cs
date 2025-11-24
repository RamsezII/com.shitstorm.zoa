using _COBRA_;
using _SGUI_;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _ZOA_
{
    public abstract partial class ShellView : MonoBehaviour
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

        public LintTheme lint_theme;
        public Shell shell;

        LintedString GetShellPrefixe() => shell.status._value switch
        {
            Shell.STATUS.BLOCKED => LintedString.EMPTY,
            _ => shell.prefixe._value,
        };

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
            stdin_field.onValueChanged.AddListener(OnStdinChanged);
            stdin_field.onSelect.AddListener(OnSelectStdin);
            stdin_field.onDeselect.AddListener(OnDeselectStdin);

            shell.Init();
            shell.on_output += AddLine;
            shell.status.AddListener(value =>
            {
                switch (value)
                {
                    case Shell.STATUS.WAIT_FOR_STDIN:
                        ResetStdin();
                        break;

                    case Shell.STATUS.BLOCKED:
                        ResetStdin();
                        break;

                    case Shell.STATUS.NETWORKING:
                        break;
                }
            });

            ResetStdin();
            RefreshStdout();
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual bool OnImguiInputs(Event e)
        {
            return false;
        }

        //----------------------------------------------------------------------------------------------------------

        protected virtual void OnDestroy()
        {
            shell?.Dispose();
            shell = null;
        }
    }
}