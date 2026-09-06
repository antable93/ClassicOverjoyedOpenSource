using GregsStack.InputSimulatorStandard.Native;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using System.Threading.Tasks;
using Wisej.Web;


namespace OverjoyedReleaseLocal
{
    public partial class Active : Page
    {
        private void Reset()
        {
            ClearCursorGlowState();
          fpsDeadZoneLookModeLatched = false;

            if (chkSafeZone.Checked)
            {
                safeZone = true;
            }
            toggleXbox = new ConcurrentBag<string>();

            toggleSwitch = new ConcurrentBag<string>();

            for (int i = 0; i < 33; i++)
            {


                inputSimulator.Keyboard.KeyUp(keyCodes[i]);



                activateToggle[i] = true; buttonPressed[i] = false;



            }


            clicked = 0;
            unclicked = 3;
            first = true;
            music.Clear();
            keyOrder = 8;
            keyOrder2 = 7;

            if (xboxMode)
            {
                for (int i = 0; i < 26; i++)
                {

                    SimGamePad.Instance.ReleaseControl(i, toggleXbox, 1);
                }
                SimGamePad.Instance.MoveSticks(0, 0, false);
                SimGamePad.Instance.MoveSticks(1, 0, false);

                SimGamePad.Instance.MoveSticks(2, 0, false);
                SimGamePad.Instance.MoveSticks(3, 0, false);

                SetCombinedXboxTrigger(1, 0);
                SetCombinedXboxTrigger(0, 0);

                SimGamePad.Instance.Update(1);
            }

            if (switchMode)
            {
                switchGamepad.ButtonA_Up();
                switchGamepad.ButtonB_Up();
                switchGamepad.ButtonX_Up();
                switchGamepad.ButtonY_Up();
                switchGamepad.ButtonL_Up();
                SetCombinedSwitchTrigger(true, false);
                switchGamepad.ButtonL3_Up();
                switchGamepad.ButtonR_Up();
                SetCombinedSwitchTrigger(false, false);
                switchGamepad.ButtonR3_Up();
                switchGamepad.ButtonDpadUp_Up();
                switchGamepad.ButtonDpadRight_Up();
                switchGamepad.ButtonDpadDown_Up();
                switchGamepad.ButtonDpadLeft_Up();
                switchGamepad.ButtonHome_Up();
                switchGamepad.ButtonPlus_Up();
                switchGamepad.ButtonMinus_Up();
                switchGamepad.ButtonCapture_Up();
                switchGamepad.leftStick_Stop();
                StopCombinedSwitchRightStick();

            }



        }

        private void HideSafeZoneOverlay()
        {
            Eval("safeZone = document.getElementById('safeZone'); if(safeZone) {safeZone.style.visibility = 'hidden';}");
        }

        private void ShowSafeZoneOverlay()
        {
            Eval("safeZone = document.getElementById('safeZone'); if(safeZone) {safeZone.style.visibility = 'visible';}");
        }

        private void ClearCursorGlowState()
        {
            Eval(@"const cursor = document.getElementById('cursorSquare');
const socket = document.getElementById('socketSquare');

const resetPressedVisualState = element => {
    if (!element) {
        return;
    }

    element.classList.remove('glow-left');
    element.classList.remove('glow-right');
    element.style.boxShadow = '';
    element.style.filter = '';
    element.style.backgroundImage = '';
    element.style.backgroundColor = '';
    element.style.borderColor = '';
    element.style.outlineColor = '';
    element.style.fill = '';
    element.style.stroke = '';

    element.querySelectorAll('*').forEach(child => {
        child.style.boxShadow = '';
        child.style.filter = '';
        child.style.backgroundImage = '';
        child.style.backgroundColor = '';
        child.style.borderColor = '';
        child.style.outlineColor = '';
        child.style.fill = '';
        child.style.stroke = '';
    });
};

resetPressedVisualState(cursor);
resetPressedVisualState(socket);");
        }

        private void drawPrompts()
        {
            var promptStateJson = JsonSerializer.Serialize(new
            {
                showLabelsHover,
                showLabelsLC,
                showLabelsRC,
                keyboardMode,
                xboxMode,
                switchMode,
                psLabels,
                chkFPS = chkFPS.Checked,
                fpsDeadZoneLookModeLatched,
                moveMouseUsingLeftClicksOnly = chkDisableClicks.Checked && IsMoveMouseUsingLeftClicksEnabled(),
                isRemoteCheckboxChecked = positionCheck,
                activeHoverQuadrant = currentQuadrant2,
                positionCheck,
                remoteX,
                remoteY,
                buttonConfig,
                diagUpRightHover,
                diagUpLeftHover,
                diagDownRightHover,
                diagDownLeftHover,
                diagUpRightLC,
                diagUpLeftLC,
                diagDownRightLC,
                diagDownLeftLC,
                diagUpRightRC,
                diagUpLeftRC,
                diagDownRightRC,
                diagDownLeftRC
            });

            Eval(
                @"(() => {
  if (!window.overjoyedPrompts) {
    return;
  }

  const nextState = " + promptStateJson + @";
  window.overjoyedPrompts.setState(nextState);
})();");
        }

        private void RefreshOverlayPointerState()
        {
            Eval(
                    @"(() => {
    const p = window.overjoyedParts;
    if (!p) {
        return;
    }

    p.updateOverlayVisibility?.();
    p.updateFromState?.();
})();");
        }

        private void SyncOverlayRemotePointerState()
        {
            Eval(
                    @"(() => {
    const p = window.overjoyedParts;
    if (!p) {
        return;
    }

    p.state.remoteX = " + remoteXoverlay + @";
    p.state.remoteY = " + remoteYoverlay + @";
    p.updateFromState?.();
    p.updateOverlayVisibility?.();
})();");
        }

      private void SetOverlayPointerPosition(int x, int y)
      {
        Eval(
            @"(() => {
    const p = window.overjoyedParts;
    if (!p || typeof p.updateSector !== 'function') {
      return;
    }

    p.updateSector(" + x + ", " + y + @");
  })();");
      }

        private void InitializePromptOverlay(int realEyePort, string promptStateJson)
        {
            InjectPromptOverlayPart1(realEyePort, promptStateJson);
            InjectPromptOverlayPart2();
            InjectPromptOverlayPart3();
            InjectPromptOverlayPart4();
            InjectPromptOverlayPart5();
        }

        private void InjectPromptOverlayPart1(int realEyePort, string promptStateJson)
        {
            Eval(
                @"(() => {
          window.overjoyedOverlay?.dispose?.();

          const p = window.overjoyedParts = {};
                p.host = this;
          p.ActivateUpper = " + ActivateUpper.ToString().ToLower() + @";
          p.ActivateMiddle = " + ActivateMiddle.ToString().ToLower() + @";
          p.ActivateLower = " + ActivateLower.ToString().ToLower() + @";

          p.BLUE = 'rgb(0, 167, 209)';
          p.WHITE = 'white';
          p.BLACK = 'black';

          p.byId = id => document.getElementById(id);
          p.container = p.byId('outerSVG');
          p.layer = p.byId('promptsLayer');
          p.svg = p.container?.querySelector('svg') || null;

          if (!p.container || !p.layer) {
            return;
          }

          p.layer.style.userSelect = 'none';
          p.layer.style.webkitUserSelect = 'none';
          p.layer.style.msUserSelect = 'none';

          p.upper = p.byId('upperDZ-inactive');
          p.middle = p.byId('middleDZ-inner');
          p.lower = p.byId('lowerDZ');

          p.x = 300;
          p.y = 350;
          p.intervalId = null;
          p.activeLabelObserver = null;
          p.baseOverlay = null;
          p.pointerdownHandler = null;
          p.pointerupHandler = null;
          p.pointercancelHandler = null;
          p.blurHandler = null;
          p.contextmenuHandler = null;
          p.mousemoveHandler = null;
          p.resizeHandler = null;

          p.promptPositions = {
            0:  { x: 300, y: 210 },
            1:  { x: 400, y: 250 },
            2:  { x: 430, y: 350 },
            3:  { x: 405, y: 450 },
            4:  { x: 300, y: 480 },
            5:  { x: 195, y: 450 },
            6:  { x: 165, y: 350 },
            7:  { x: 200, y: 250 },
            8:  { x: 210, y: 240 },
            9:  { x: 152, y: 240 },
            10: { x: 450, y: 240 },
            11: { x: 392, y: 240 },
            12: { x: 210, y: 456 },
            13: { x: 152, y: 456 },
            14: { x: 450, y: 456 },
            15: { x: 392, y: 456 }
          };

          p.labelPositions = {
            0: { x: 300, y: 307 },
            1: { x: 300, y: 350 },
            2: { x: 300, y: 395 }
          };

          p.state = {
            assetBase: 'http://localhost:" + realEyePort + @"/images/prompts',
            showLabelsHover: false,
            showLabelsLC: false,
            showLabelsRC: false,
            keyboardMode: false,
            xboxMode: true,
            switchMode: false,
            psLabels: false,
            chkFPS: false,
            moveMouseUsingLeftClicksOnly: false,
                        isRemoteCheckboxChecked: false,
            positionCheck: false,
            remoteX: 0,
            remoteY: 0,
            buttonConfig: [],
            diagUpRightHover: false,
            diagUpLeftHover: false,
            diagDownRightHover: false,
            diagDownLeftHover: false,
            diagUpRightLC: false,
            diagUpLeftLC: false,
            diagDownRightLC: false,
            diagDownLeftLC: false,
            diagUpRightRC: false,
            diagUpLeftRC: false,
            diagDownRightRC: false,
            diagDownLeftRC: false,
            activeHoverQuadrant: null,
            lastEnteredHoverQuadrant: null,
            heldLcQuadrant: null,
            lcPointerDown: false,
            rcPointerDown: false,
            lcTransitionPending: false,
            highlightToggleStates: Object.create(null),
            highlightOnceTimeouts: Object.create(null),
            lcToggleCycles: Object.create(null),
            rcToggleCycles: Object.create(null),
            lcHoldStartedAt: 0,
            lcMinHighlightMs: 250,
            lcHeldReleaseTimeout: null,
            hoverFillRatio: 0,
            exitHoldStartedAt: 0,
            exitHoldDurationMs: 2000,
            exitHoldRatio: 0,
            exitHoldTriggered: false,
            lcPressQuadrant: null,
            lcPressTriggerArmed: false,
            lcRegularHoldAllowed: false,
            lcPressType: null,
                        rcPressQuadrant: null,
            rcPressType: null
          };

          Object.assign(p.state, " + promptStateJson + @");

          p.pctX = v => `${(v / 600) * 100}%`;
          p.pctY = v => `${(v / 600) * 100}%`;

                    p.ensureLblActiveElement = () => {
                        if (p.lblActiveDom && p.lblActiveDom.isConnected) {
                            return p.lblActiveDom;
                        }

                        const label = p.byId('lblActiveOverlay');
                        if (!label) {
                            return null;
                        }

                        label.style.position = 'absolute';
                        label.style.margin = '0';
                        label.style.boxSizing = 'border-box';
                        label.style.userSelect = 'none';
                        label.style.webkitUserSelect = 'none';
                        label.style.msUserSelect = 'none';
                        label.style.zIndex = '40';
                        label.classList.add('ActiveZone');
                        label.textContent = 'Overjoyed is inactive. Hold Click Here to activate!';
                        label.dataset.overjoyedState = 'inactive';
                        label.classList.add('overjoyed-active-state-inactive');
                        label.hidden = false;

                        p.lblActiveDom = label;

                        return label;
                    };

                    p.getLblActive = () => p.ensureLblActiveElement();

                    p.applyLblActiveState = state => {
                        const label = p.getLblActive();
                        if (!label) return;

                        const isActive = state === 'active';
                        label.dataset.overjoyedState = isActive ? 'active' : 'inactive';
                        label.classList.toggle('overjoyed-active-state-active', isActive);
                        label.classList.toggle('overjoyed-active-state-inactive', !isActive);
                    };

                    p.syncLblActiveStateFromDom = () => {
                        const label = p.getLblActive();
                        if (!label) return null;

                        const text = String(label.textContent || '').toLowerCase();
                        const nextState = text.includes('inactive') ? 'inactive' : 'active';
                        if (label.dataset.overjoyedState !== nextState) {
                            label.dataset.overjoyedState = nextState;
                        }
                        return label.dataset.overjoyedState;
                    };

                    p.setLblActiveVisible = visible => {
                        const label = p.getLblActive();
                        if (!label) return;

                        label.hidden = !visible;
                    };

                    p.setLblActiveText = text => {
                        const label = p.getLblActive();
                        if (!label) return;

                        label.textContent = String(text || '');
                    };

                    p.setLblActiveTop = _topPx => {
                    };

                    p.setLblActiveInactive = (text, _topPx) => {
                        const label = p.getLblActive();
                        if (!label) return;

                        label.hidden = false;
                        label.textContent = String(text || '');
                        p.applyLblActiveState('inactive');
                    };

                    p.setLblActiveActive = text => {
                        const label = p.getLblActive();
                        if (!label) return;

                        label.hidden = false;
                        label.textContent = String(text || '');
                        p.applyLblActiveState('active');
                    };

                    p.isLblActiveBottomAnchored = () => {
                        const label = p.getLblActive();
                        if (!label) return false;

                        return label.classList.contains('overjoyed-active-state-active');
                    };

                    p.isFocusMouseChecked = () => {
                        const focusMouseCheckbox =
                            document.querySelector('div[name=""chkKeepMouseInside""]') ||
                            document.querySelector('[name=""chkKeepMouseInside""]') ||
                            document.getElementById('chkKeepMouseInside');

                        if (!focusMouseCheckbox) return false;

                        return focusMouseCheckbox.classList?.contains('qx-checkbox-checked') ||
                            focusMouseCheckbox.getAttribute?.('aria-checked') === 'true' ||
                            focusMouseCheckbox.checked === true;
                    };

                    p.cancelLblActiveHoldAnimation = () => {
                        const label = p.getLblActive();

                        if (window.overjoyedLblActiveHoldTimeout) {
                            clearTimeout(window.overjoyedLblActiveHoldTimeout);
                            window.overjoyedLblActiveHoldTimeout = null;
                        }

                        if (!label) {
                            return;
                        }

                        label.classList.remove('overjoyed-lblactive-hold-fill');
                        label.style.animation = 'none';
                        void label.offsetWidth;
                        label.style.animation = '';
                    };

                    p.startLblActiveHoldAnimation = () => {
                        const label = p.getLblActive();
                        if (!label) return;

                        p.cancelLblActiveHoldAnimation();

                        label.classList.remove('overjoyed-lblactive-hold-fill');
                        void label.offsetWidth;
                        label.classList.add('overjoyed-lblactive-hold-fill');

                        if (window.overjoyedLblActiveHoldTimeout) {
                            clearTimeout(window.overjoyedLblActiveHoldTimeout);
                        }

                        window.overjoyedLblActiveHoldTimeout = window.setTimeout(() => {
                            window.overjoyedLblActiveHoldTimeout = null;

                            if (!label.classList.contains('overjoyed-lblactive-hold-fill')) {
                                return;
                            }

                            if (p.host && typeof p.host.CompleteLblActiveHoldActivate === 'function') {
                                p.host.CompleteLblActiveHoldActivate();
                            }
                        }, 2000);
                    };

                    p.lblActiveMouseOverHandler = () => {
                        if (!p.canShowHighlights()) {
                            return;
                        }

                        p.setLblActiveText('Double Click to Use Emergency Escape Hatch');
                    };

                    p.lblActiveMouseLeaveHandler = e => {
                        if (p.isLblActiveInactiveState()) {
                            p.cancelLblActiveHoldAnimation();
                            return;
                        }

                        p.setLblActiveActive(p.getActiveLblActiveMessage());

                        const label = p.getLblActive();
                        if (!label) {
                            return;
                        }

                        const rect = label.getBoundingClientRect();
                        const exitedThroughTop = !!e && typeof e.clientY === 'number' && e.clientY <= (rect.top + 1);

                        if (exitedThroughTop) {
                            return;
                        }

                        if (!p.isFocusMouseChecked() && !p.state.positionCheck && p.host && typeof p.host.TriggerEscapeHatchDeactivate === 'function') {
                            p.host.TriggerEscapeHatchDeactivate();
                        }
                    };

                    p.lblActiveMouseDownHandler = e => {
                        e?.preventDefault?.();
                        e?.stopPropagation?.();
                        e?.stopImmediatePropagation?.();

                        if (!(e.button === 0 || e.button === 2) || p.state.positionCheck || !p.isLblActiveInactiveState()) {
                            return;
                        }

                        p.startLblActiveHoldAnimation();
                    };

                    p.lblActiveMouseUpHandler = e => {
                        e?.preventDefault?.();
                        e?.stopPropagation?.();
                        e?.stopImmediatePropagation?.();

                        if (!(e.button === 0 || e.button === 2)) {
                            return;
                        }

                        p.cancelLblActiveHoldAnimation();
                    };

                    p.lblActiveClickHandler = e => {
                        e?.preventDefault?.();
                        e?.stopPropagation?.();
                        e?.stopImmediatePropagation?.();
                    };

                    p.lblActiveDoubleClickHandler = e => {
                        e?.preventDefault?.();
                        e?.stopPropagation?.();
                        e?.stopImmediatePropagation?.();

                        if (!p.canShowHighlights() || p.state.positionCheck) {
                            return;
                        }

                        if (p.host && typeof p.host.TriggerEscapeHatchDoubleClickDeactivate === 'function') {
                            p.host.TriggerEscapeHatchDoubleClickDeactivate();
                            return;
                        }

                        if (p.host && typeof p.host.TriggerEscapeHatchDeactivate === 'function') {
                            p.host.TriggerEscapeHatchDeactivate();
                        }
                    };

                    p.attachLblActiveEventHandlers = () => {
                        const label = p.getLblActive();
                        if (!label || p.lblActiveElement === label) {
                            return;
                        }

                        if (p.lblActiveElement) {
                            p.detachLblActiveEventHandlers();
                        }

                        label.addEventListener('mouseover', p.lblActiveMouseOverHandler);
                        label.addEventListener('mouseleave', p.lblActiveMouseLeaveHandler);
                        label.addEventListener('mouseout', p.lblActiveMouseLeaveHandler);
                        label.addEventListener('mousedown', p.lblActiveMouseDownHandler);
                        label.addEventListener('mouseup', p.lblActiveMouseUpHandler);
                        label.addEventListener('click', p.lblActiveClickHandler);
                        label.addEventListener('dblclick', p.lblActiveDoubleClickHandler);
                        p.lblActiveElement = label;
                    };

                    p.detachLblActiveEventHandlers = () => {
                        const label = p.lblActiveElement;
                        if (!label) {
                            return;
                        }

                        label.removeEventListener('mouseover', p.lblActiveMouseOverHandler);
                        label.removeEventListener('mouseleave', p.lblActiveMouseLeaveHandler);
                        label.removeEventListener('mouseout', p.lblActiveMouseLeaveHandler);
                        label.removeEventListener('mousedown', p.lblActiveMouseDownHandler);
                        label.removeEventListener('mouseup', p.lblActiveMouseUpHandler);
                        label.removeEventListener('click', p.lblActiveClickHandler);
                        label.removeEventListener('dblclick', p.lblActiveDoubleClickHandler);
                        p.lblActiveElement = null;
                    };

          p.applyDefaultFills = () => {
                        if (p.upper) p.upper.style.fill = 'none';
                        if (p.middle) p.middle.style.fill = 'none';
                        if (p.lower) p.lower.style.fill = 'none';
          };

                    p.ensureActivationProgressDefs = () => {
                        if (!p.svg) return;

                        let defs = p.svg.querySelector('#overjoyedActivationProgressDefs');
                        if (!defs) {
                            defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
                            defs.setAttribute('id', 'overjoyedActivationProgressDefs');
                            p.svg.insertBefore(defs, p.svg.firstChild || null);
                        }

                        const gradientSpecs = [
                            { id: 'overjoyedUpperHoldGradient', tag: 'linearGradient', attrs: { x1: '0%', y1: '0%', x2: '100%', y2: '0%' } },
                            { id: 'overjoyedMiddleHoldGradient', tag: 'linearGradient', attrs: { x1: '0%', y1: '0%', x2: '100%', y2: '0%' } },
                            { id: 'overjoyedLowerHoldGradient', tag: 'linearGradient', attrs: { x1: '100%', y1: '0%', x2: '0%', y2: '0%' } }
                        ];

                        gradientSpecs.forEach(spec => {
                            let gradient = defs.querySelector(`#${spec.id}`);
                            if (!gradient) {
                                gradient = document.createElementNS('http://www.w3.org/2000/svg', spec.tag);
                                gradient.setAttribute('id', spec.id);
                                Object.entries(spec.attrs).forEach(([name, value]) => gradient.setAttribute(name, value));

                                const start = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
                                start.setAttribute('class', 'overjoyed-progress-start');
                                start.setAttribute('offset', '0%');
                                start.setAttribute('stop-color', p.BLUE);
                                gradient.appendChild(start);

                                const end = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
                                end.setAttribute('class', 'overjoyed-progress-end');
                                end.setAttribute('offset', '0%');
                                end.setAttribute('stop-color', 'transparent');
                                gradient.appendChild(end);

                                defs.appendChild(gradient);
                            }
                        });
                    };

                    p.setActivationProgressFill = (element, gradientId, ratio) => {
                        if (!element) return;

                        const clampedRatio = Math.max(0, Math.min(1, Number(ratio) || 0));
                        if (clampedRatio <= 0) {
                            element.style.fill = 'none';
                            return;
                        }

                        p.ensureActivationProgressDefs();

                        const gradient = p.svg?.querySelector(`#${gradientId}`);
                        if (!gradient) {
                            element.style.fill = clampedRatio >= 1 ? p.BLUE : 'none';
                            return;
                        }

                        const percent = `${clampedRatio * 100}%`;
                        const start = gradient.querySelector('.overjoyed-progress-start');
                        const end = gradient.querySelector('.overjoyed-progress-end');
                        if (start) start.setAttribute('offset', percent);
                        if (end) end.setAttribute('offset', percent);

                        element.style.fill = `url(#${gradientId})`;
                    };

                    p.paintActivationZoneProgress = ratio => {
                        const slot = p.getActivationDeadZoneLabelSlot();

                        p.applyDefaultFills();

                        if (slot === 0) {
                            p.setActivationProgressFill(p.upper, 'overjoyedUpperHoldGradient', ratio);
                            return;
                        }

                        if (slot === 1) {
                            p.setActivationProgressFill(p.middle, 'overjoyedMiddleHoldGradient', ratio);
                            return;
                        }

                        if (slot === 2) {
                            p.setActivationProgressFill(p.lower, 'overjoyedLowerHoldGradient', ratio);
                        }
                    };

                    p.ensureActivationLabelLayer = () => {
                        if (p.activationLabelLayer && p.activationLabelLayer.isConnected) {
                            return p.activationLabelLayer;
                        }

                        const layer = p.byId('activationPromptLabelsLayer');
                        if (!layer) {
                            return null;
                        }

                        layer.style.userSelect = 'none';
                        layer.style.webkitUserSelect = 'none';
                        layer.style.msUserSelect = 'none';

                        p.activationLabelLayer = layer;
                        return layer;
                    };

                    p.getPromptLabelElement = slot => {
                        const selector = `.prompt-label[data-slot='${slot}']`;
                      const activationLabel = p.ensureActivationLabelLayer()?.querySelector(selector) || null;
                      const regularLabel = p.layer.querySelector(selector);

                      if (!activationLabel) {
                        return regularLabel;
                      }

                      if (!regularLabel) {
                        return activationLabel;
                      }

                      const activationStyle = window.getComputedStyle(activationLabel);
                      if (activationStyle.display !== 'none' && activationStyle.visibility !== 'hidden') {
                        return activationLabel;
                      }

                      return regularLabel;
                    };

                    p.getPromptBoxElement = slot =>
                        p.layer.querySelector(`.prompt-box[data-slot='${slot}']`);

                    p.getRegularPromptLabelElement = slot =>
                        p.layer.querySelector(`.prompt-label[data-slot='${slot}']`);

                    p.getActivationLabelElement = slot =>
                        p.ensureActivationLabelLayer()?.querySelector(`.prompt-label[data-slot='${slot}']`) || null;

                    p.getAllPromptElements = () => [
                        ...p.layer.querySelectorAll('.prompt-box, .prompt-label'),
                        ...(p.ensureActivationLabelLayer()?.querySelectorAll('.prompt-label') || [])
                    ];

                    p.getActivationLabelElements = () =>
                        p.ensureActivationLabelLayer()?.querySelectorAll('.exit-hold-label') || [];

                    p.eyeLabelDoubleClickHandler = e => {
                      e?.preventDefault?.();
                      e?.stopPropagation?.();
                      e?.stopImmediatePropagation?.();

                      if (!p.canShowHighlights() || !p.state.chkFPS || !p.state.showLabelsLC) {
                        return;
                      }

                      if (p.host && typeof p.host.ToggleFpsDeadZoneLookModeFromPrompt === 'function') {
                        p.host.ToggleFpsDeadZoneLookModeFromPrompt();
                      }
                    };

                    p.bindPromptLabelInteraction = (el, item) => {
                      if (!el) {
                        return;
                      }

                      el.removeEventListener('dblclick', p.eyeLabelDoubleClickHandler);

                      const isInteractiveEye = !!item && !!item.eye && !item.isActivationLabel;
                      el.style.pointerEvents = isInteractiveEye ? 'auto' : 'none';

                      if (isInteractiveEye) {
                        el.addEventListener('dblclick', p.eyeLabelDoubleClickHandler);
                      }
                    };

                    p.isElementVisible = el =>
                        !!el && window.getComputedStyle(el).display !== 'none' && window.getComputedStyle(el).visibility !== 'hidden';

                    p.getPointerHalfSize = pointerEl => {
                        if (!pointerEl) return 20;

                        const rect = pointerEl.getBoundingClientRect();
                        const size = Math.max(rect.width || 0, rect.height || 0);
                        if (size > 0) {
                            return size / 2;
                        }

                        const computed = window.getComputedStyle(pointerEl);
                        const fallbackSize = Math.max(parseFloat(computed.width) || 0, parseFloat(computed.height) || 0, 40);
                        return fallbackSize / 2;
                    };

                    p.syncPointerElements = () => {
                        const cursor = p.byId('cursorSquare');
                        if (cursor) {
                            const cursorHalf = p.getPointerHalfSize(cursor);
                            cursor.style.left = `${p.x - cursorHalf}px`;
                            cursor.style.top = `${p.y - cursorHalf}px`;
                        }

                        const socket = p.byId('socketSquare');
                        if (socket) {
                            socket.style.left = `${p.state.remoteX || 0}px`;
                            socket.style.top = `${p.state.remoteY || 0}px`;
                        }
                    };

          p.clearPromptLayer = () => {
                        p.layer.querySelectorAll('.prompt-box').forEach(el => {
                            el.style.display = 'none';
                            el.style.left = '';
                            el.style.top = '';
                            el.classList.remove('overjoyed-highlighted');

                            const fill = el.querySelector('.prompt-fill');
                            if (fill) {
                                fill.style.backgroundImage = '';
                            }

                            const img = el.querySelector('img');
                            if (img) {
                                img.removeAttribute('src');
                                delete img.dataset.fallbackApplied;
                            }
                        });

                        p.layer.querySelectorAll('.prompt-label').forEach(el => {
                          el.removeEventListener('dblclick', p.eyeLabelDoubleClickHandler);
                            el.style.display = 'none';
                            el.style.visibility = '';
                            el.style.left = '';
                            el.style.top = '';
                            el.style.backgroundImage = '';
                            el.style.backgroundColor = '';
                            el.style.color = '';
                            el.classList.remove('overjoyed-highlighted', 'overjoyed-eye-label');
                            if (!el.classList.contains('exit-hold-label')) {
                                el.textContent = '';
                            }
                        });

                        p.getActivationLabelElements().forEach(el => {
                            el.style.display = 'none';
                            el.style.visibility = 'hidden';
                            el.style.left = '';
                            el.style.top = '';
                            el.style.backgroundImage = '';
                            el.style.backgroundColor = '';
                            el.style.color = '';
                            el.classList.remove('overjoyed-highlighted');
                        });
          };

          p.clearLcHeldHighlight = () => {
            p.state.heldLcQuadrant = null;
          };

          p.cancelLcHeldRelease = () => {
            if (p.state.lcHeldReleaseTimeout) {
              clearTimeout(p.state.lcHeldReleaseTimeout);
              p.state.lcHeldReleaseTimeout = null;
            }
          };

          p.normalizeKeyboardPromptName = value => {
            const key = String(value || '').toUpperCase();
            if (key === 'DIVIDE') return 'SLASH';
            if (key === 'MULTIPLY') return 'ASTERISK';
            if (key === 'SUBTRACT') return 'MINUS';
            if (key === 'ADD') return 'PLUS';
            return key;
          };

          p.getModeNonePromptPath = () => {
            if (p.state.keyboardMode) return `${p.state.assetBase}/Keyboard/None_Key_Dark.png`;
            if (p.state.xboxMode) {
              return p.state.psLabels
                ? `${p.state.assetBase}/PS5/PS5_NONE.png`
                : `${p.state.assetBase}/Xbox Series/XboxSeriesX_NONE.png`;
            }
            return `${p.state.assetBase}/Switch/Switch_NONE.png`;
          };

          p.getPromptImagePath = configIndex => {
            const row = p.state.buttonConfig[configIndex] || [];

            if (p.state.keyboardMode) {
              const key = p.normalizeKeyboardPromptName(row[0]);
              return `${p.state.assetBase}/Keyboard/${key}_Key_Dark.png`;
            }

            if (p.state.xboxMode) {
              const value = String(row[3] || '').toUpperCase();
              return p.state.psLabels
                ? `${p.state.assetBase}/PS5/PS5_${value}.png`
                : `${p.state.assetBase}/Xbox Series/XboxSeriesX_${value}.png`;
            }

            const value = String(row[6] || '').toUpperCase().replace(/\s+/g, '');
            return `${p.state.assetBase}/Switch/Switch_${value}.png`;
          };

          p.isPromptHidden = configIndex =>
            String(p.state.buttonConfig[configIndex]?.[2] || '').toLowerCase() === 'true';

        })();");
        }

        private void InjectPromptOverlayPart2()
        {
            Debug.WriteLine("[InjectPromptOverlayPart2] Injecting prompt overlay helpers.");

            Eval(
                @"(() => {
          const p = window.overjoyedParts;
          if (!p) return;

                    p.debugPart2 = (eventName, details) => {
                        try {
                            const store = window.overjoyedPromptDebugLog = window.overjoyedPromptDebugLog || [];
                            const entry = {
                                timestamp: new Date().toISOString(),
                                eventName,
                                details: details || null
                            };

                            store.push(entry);
                            if (store.length > 200) {
                                store.splice(0, store.length - 200);
                            }

                            console.log('[Overjoyed][PromptOverlayPart2]', eventName, details || '');
                        } catch (error) {
                            console.warn('[Overjoyed][PromptOverlayPart2][debug-failed]', error);
                        }
                    };

                    window.overjoyedPromptDebug = p.debugPart2;

                    p.debugPart2('init', {
                        showLabelsHover: !!p.state?.showLabelsHover,
                        showLabelsLC: !!p.state?.showLabelsLC,
                        showLabelsRC: !!p.state?.showLabelsRC,
                        xboxMode: !!p.state?.xboxMode,
                        switchMode: !!p.state?.switchMode,
                        psLabels: !!p.state?.psLabels,
                        activateUpper: !!p.ActivateUpper,
                        activateMiddle: !!p.ActivateMiddle,
                        activateLower: !!p.ActivateLower,
                        buttonConfigLength: Array.isArray(p.state?.buttonConfig) ? p.state.buttonConfig.length : -1
                    });

          p.getActivePromptBaseIndex = () => {
                        const result = p.state.showLabelsHover ? 0 : p.state.showLabelsLC ? 8 : 16;
                        p.debugPart2('getActivePromptBaseIndex', {
                            result,
                            showLabelsHover: !!p.state.showLabelsHover,
                            showLabelsLC: !!p.state.showLabelsLC,
                            showLabelsRC: !!p.state.showLabelsRC
                        });
                        return result;
          };

          p.isDiagonalEnabled = slot => {
                        let result;

            if (p.state.showLabelsHover) {
                            result = {
                1: p.state.diagUpRightHover,
                3: p.state.diagDownRightHover,
                5: p.state.diagDownLeftHover,
                7: p.state.diagUpLeftHover
                            }[slot] || false;
                        } else if (p.state.showLabelsLC) {
                            result = {
                                1: p.state.diagUpRightLC,
                                3: p.state.diagDownRightLC,
                                5: p.state.diagDownLeftLC,
                                7: p.state.diagUpLeftLC
                            }[slot] || false;
                        } else {
                            result = {
                                1: p.state.diagUpRightRC,
                                3: p.state.diagDownRightRC,
                                5: p.state.diagDownLeftRC,
                                7: p.state.diagUpLeftRC
                            }[slot] || false;
            }

                        p.debugPart2('isDiagonalEnabled', {
                            slot,
                            result,
                            mode: p.state.showLabelsHover ? 'hover' : p.state.showLabelsLC ? 'lc' : 'rc'
                        });
                        return result;
          };

          p.getDiagonalLayout = (slot, prevIndex, nextIndex) => {
                        let result = null;

                        switch (slot) {
                            case 1: result = { firstAuxSlot: 10, firstConfigIndex: prevIndex, secondAuxSlot: 11, secondConfigIndex: nextIndex }; break;
                            case 3: result = { firstAuxSlot: 14, firstConfigIndex: nextIndex, secondAuxSlot: 15, secondConfigIndex: prevIndex }; break;
                            case 5: result = { firstAuxSlot: 12, firstConfigIndex: prevIndex, secondAuxSlot: 13, secondConfigIndex: nextIndex }; break;
                            case 7: result = { firstAuxSlot: 8, firstConfigIndex: nextIndex, secondAuxSlot: 9, secondConfigIndex: prevIndex }; break;
                        }

                        p.debugPart2('getDiagonalLayout', { slot, prevIndex, nextIndex, result });
                        return result;
          };

          p.getHorizontalStickDirection = value => {
            value = String(value || '').toUpperCase();
            if (value.includes(' RIGHT')) return 'Right';
            if (value.includes(' LEFT')) return 'Left';
            return null;
          };

          p.getVerticalStickDirection = value => {
            value = String(value || '').toUpperCase();
            if (value.includes(' UP')) return 'Up';
            if (value.includes(' DOWN')) return 'Down';
            return null;
          };

          p.tryGetMergedStickSuffix = (slot, first, second) => {
            const a = String(first || '').trim().toUpperCase();
            const b = String(second || '').trim().toUpperCase();

            let stickPrefix = null;
            if (a.startsWith('LS ') && b.startsWith('LS ')) stickPrefix = 'LS';
            else if (a.startsWith('RS ') && b.startsWith('RS ')) stickPrefix = 'RS';
                        else {
                            p.debugPart2('tryGetMergedStickSuffix:no-match', { slot, first: a, second: b });
                            return null;
                        }

            const horizontal = p.getHorizontalStickDirection(a) || p.getHorizontalStickDirection(b);
            const vertical = p.getVerticalStickDirection(a) || p.getVerticalStickDirection(b);
                        if (!horizontal || !vertical) {
                            p.debugPart2('tryGetMergedStickSuffix:missing-direction', {
                                slot,
                                first: a,
                                second: b,
                                horizontal,
                                vertical
                            });
                            return null;
                        }

                        let result = null;
                        switch (slot) {
                            case 1: result = horizontal === 'Right' && vertical === 'Up' ? { stickPrefix, suffix: 'RightUp' } : null; break;
                            case 3: result = horizontal === 'Right' && vertical === 'Down' ? { stickPrefix, suffix: 'RightDown' } : null; break;
                            case 5: result = horizontal === 'Left' && vertical === 'Down' ? { stickPrefix, suffix: 'LeftDown' } : null; break;
                            case 7: result = horizontal === 'Left' && vertical === 'Up' ? { stickPrefix, suffix: 'LeftUp' } : null; break;
                        }

                        p.debugPart2('tryGetMergedStickSuffix', {
                            slot,
                            first: a,
                            second: b,
                            horizontal,
                            vertical,
                            result
                        });
                        return result;
          };

          p.tryGetMergedPromptPath = (slot, firstConfigIndex, secondConfigIndex) => {
                        let result = null;

            if (p.state.xboxMode) {
              const first = p.state.buttonConfig[firstConfigIndex]?.[3];
              const second = p.state.buttonConfig[secondConfigIndex]?.[3];
              const merged = p.tryGetMergedStickSuffix(slot, first, second);
              if (merged) {
                                result = p.state.psLabels
                  ? `${p.state.assetBase}/PS5/PS5_${merged.stickPrefix} ${merged.suffix}.png`
                  : `${p.state.assetBase}/Xbox Series/XboxSeriesX_${merged.stickPrefix} ${merged.suffix}.png`;
                                p.debugPart2('tryGetMergedPromptPath', {
                                    slot,
                                    mode: p.state.psLabels ? 'ps5' : 'xbox',
                                    firstConfigIndex,
                                    secondConfigIndex,
                                    result
                                });
                                return result;
              }
            }

            if (p.state.switchMode) {
              const first = p.state.buttonConfig[firstConfigIndex]?.[6];
              const second = p.state.buttonConfig[secondConfigIndex]?.[6];
              const merged = p.tryGetMergedStickSuffix(slot, first, second);
              if (merged) {
                                result = `${p.state.assetBase}/Switch/Switch_${merged.stickPrefix}${merged.suffix}.png`;
                                p.debugPart2('tryGetMergedPromptPath', {
                                    slot,
                                    mode: 'switch',
                                    firstConfigIndex,
                                    secondConfigIndex,
                                    result
                                });
                                return result;
              }
            }

                        p.debugPart2('tryGetMergedPromptPath:none', { slot, firstConfigIndex, secondConfigIndex });
                        return null;
          };

          p.getDeadZoneLabelText = configIndex => {
            const row = p.state.buttonConfig[configIndex] || [];

                        let result;

            if (p.state.switchMode) {
                            result = String(row[6] || '')
                .toUpperCase()
                .replace(/NONE/g, '')
                .replace(/ BUTTON/g, '')
                .replace(/ LEFT/g, ' ?')
                .replace(/ RIGHT/g, ' ?')
                .replace(/ UP/g, ' ?')
                .replace(/ DOWN/g, ' ?')
                .replace(/Plus \(Start\)/g, '(+)')
                .replace(/Minus \(Select\)/g, '(-)');
                            p.debugPart2('getDeadZoneLabelText', { configIndex, mode: 'switch', result });
                            return result;
            }

            if (p.state.xboxMode) {
              let text = String(row[3] || '')
                .toUpperCase()
                .replace(/NONE/g, '')
                .replace(/ BUTTON/g, '')
                .replace(/ LEFT/g, ' ?')
                .replace(/ RIGHT/g, ' ?')
                .replace(/ UP/g, ' ?')
                .replace(/ DOWN/g, ' ?');

              if (p.state.psLabels) {
                                result = text
                  .replace(/A OR /g, '')
                  .replace(/B OR /g, '')
                  .replace(/X OR /g, '')
                  .replace(/Y OR /g, '')
                  .replace(/LB OR /g, '')
                  .replace(/LT OR /g, '')
                  .replace(/LSB OR /g, '')
                  .replace(/RB OR /g, '')
                  .replace(/RT OR /g, '')
                  .replace(/RSB OR /g, '')
                  .replace(/MENU OR /g, '')
                  .replace(/VIEW OR /g, '');
                                p.debugPart2('getDeadZoneLabelText', { configIndex, mode: 'ps', result });
                                return result;
              }

                            result = text
                .replace(/ OR R1/g, '')
                .replace(/ OR R2/g, '')
                .replace(/ OR L1/g, '')
                .replace(/ OR L2/g, '')
                .replace(/ OR X/g, '')
                .replace(/ OR O/g, '')
                .replace(/ OR ?/g, '')
                .replace(/ OR ?/g, '')
                .replace(/ OR L3/g, '')
                .replace(/ OR R3/g, '')
                .replace(/ OR START/g, '')
                .replace(/ OR SELECT/g, '');
              p.debugPart2('getDeadZoneLabelText', { configIndex, mode: 'xbox', result });
              return result;
            }

            result = String(row[0] || '').toUpperCase().replace(/NONE/g, '').replace(/BACKTICK/g, '');
            p.debugPart2('getDeadZoneLabelText', { configIndex, mode: 'keyboard', result });
            return result;
          };

          p.getDeadZoneLabelConfigIndices = () => {
            const result = p.state.showLabelsHover ? [24, 27, 30] : p.state.showLabelsLC ? [25, 28, 31] : [26, 29, 32];
            p.debugPart2('getDeadZoneLabelConfigIndices', { result });
            return result;
          };

                    p.getActivationDeadZoneLabelSlot = () => {
                        const result = p.ActivateUpper ? 0 : p.ActivateMiddle ? 1 : p.ActivateLower ? 2 : null;
                        p.debugPart2('getActivationDeadZoneLabelSlot', { result });
                        return result;
                    };

                    p.getActivationDeadZoneQuadrant = () => {
                        const slot = p.getActivationDeadZoneLabelSlot();
                        const result = slot === 0 ? 24 : slot === 1 ? 27 : slot === 2 ? 30 : null;
                        p.debugPart2('getActivationDeadZoneQuadrant', { slot, result });
                        return result;
                    };

                    p.getActivationDeadZoneLabelClass = slot => {
                        const result = slot === 0
                            ? 'exit-hold-upper'
                            : slot === 1
                                ? 'exit-hold-middle'
                                : slot === 2
                                    ? 'exit-hold-lower'
                                    : '';
                        p.debugPart2('getActivationDeadZoneLabelClass', { slot, result });
                        return result;
                    };

          p.debugPart2('init:complete');

        })();");
        }

        private void InjectPromptOverlayPart3()
        {
            Eval(
                @"(() => {
          const p = window.overjoyedParts;
          if (!p) return;

          p.buildPromptModel = () => {
            const visible = p.state.showLabelsHover || p.state.showLabelsLC || p.state.showLabelsRC;
            if (!visible) return { visible: false, boxes: [], labels: [] };

            const base = p.getActivePromptBaseIndex();
            const boxes = Array.from({ length: 16 }, (_, slot) => ({
              slot,
              visible: false,
              src: '',
              fallback: p.getModeNonePromptPath()
            }));

            function setSlot(slot, configIndex) {
              boxes[slot].src = p.getPromptImagePath(configIndex);
              boxes[slot].visible = !p.isPromptHidden(configIndex);
            }

            setSlot(0, base + 0);
            setSlot(2, base + 2);
            setSlot(4, base + 4);
            setSlot(6, base + 6);

            [1, 3, 5, 7].forEach(slot => {
              const configIndex = base + slot;
              const prevIndex = base + slot - 1;
              const nextIndex = base + ((slot + 1) % 8);
              const layout = p.getDiagonalLayout(slot, prevIndex, nextIndex);
              if (!layout) return;

              boxes[slot].visible = false;
              boxes[layout.firstAuxSlot].visible = false;
              boxes[layout.secondAuxSlot].visible = false;

              if (!p.isDiagonalEnabled(slot)) {
                setSlot(slot, configIndex);
                return;
              }

              const merged = p.tryGetMergedPromptPath(slot, prevIndex, nextIndex);
              if (merged) {
                boxes[slot].src = merged;
                boxes[slot].visible = true;
                return;
              }

              setSlot(layout.firstAuxSlot, layout.firstConfigIndex);
              setSlot(layout.secondAuxSlot, layout.secondConfigIndex);
            });

                        const labelConfigIndices = p.getDeadZoneLabelConfigIndices();
                        const activationLabelSlot = p.getActivationDeadZoneLabelSlot();

            const labels = [0, 1, 2].map(labelSlot => {
              const configIndex = labelConfigIndices[labelSlot];
                            const isActivationLabel = labelSlot === activationLabelSlot;
              const eye = p.state.chkFPS && p.state.showLabelsLC && labelSlot === 1;
              const eyeToggled = eye && !!p.state.fpsDeadZoneLookModeLatched;

              return {
                slot: labelSlot,
                                visible: isActivationLabel || !p.isPromptHidden(configIndex),
                                text: isActivationLabel ? 'EXIT (Hold)' : (eye ? '👁️' : p.getDeadZoneLabelText(configIndex)),
                                                                eye: !isActivationLabel && eye,
                                                                eyeToggled: !isActivationLabel && eyeToggled,
                                                                                                                                isActivationLabel,
                                                                                                                                zIndex: isActivationLabel ? 40 : 20
              };
            });

            return { visible: true, boxes, labels };
          };

          p.renderPrompts = () => {
            p.clearPromptLayer();

            const model = p.buildPromptModel();
            if (!model.visible) {
              return;
            }

            for (const box of model.boxes) {
              if (!box.visible) continue;
              const pos = p.promptPositions[box.slot];
              if (!pos) continue;

                            const el = p.getPromptBoxElement(box.slot);
                            if (!el) continue;

              el.style.left = p.pctX(pos.x);
              el.style.top = p.pctY(pos.y);
                            el.style.display = 'block';
              el.style.userSelect = 'none';
              el.style.webkitUserSelect = 'none';
              el.style.msUserSelect = 'none';
              el.setAttribute('unselectable', 'on');

                            const img = el.querySelector('img');
                            if (!img) continue;

                            delete img.dataset.fallbackApplied;
              img.src = box.src;
              img.draggable = false;
              img.onerror = function () {
                if (this.dataset.fallbackApplied === 'true') return;
                this.dataset.fallbackApplied = 'true';
                this.src = box.fallback;
              };
            }

            for (const item of model.labels) {
              if (!item.visible) continue;
              const pos = p.labelPositions[item.slot];
              if (!pos) continue;

                            const el = item.isActivationLabel
                                ? p.getActivationLabelElement(item.slot)
                                : p.getRegularPromptLabelElement(item.slot);
                            if (!el) continue;

              el.style.left = p.pctX(pos.x);
              el.style.top = p.pctY(pos.y);
              el.style.zIndex = String(item.zIndex || 20);
                            el.style.display = 'flex';
              el.style.userSelect = 'none';
              el.style.webkitUserSelect = 'none';
              el.style.msUserSelect = 'none';
              el.setAttribute('unselectable', 'on');

                            if (!item.isActivationLabel) {
                                el.textContent = item.text;
                            }

                            el.classList.toggle('overjoyed-eye-label', !!item.eye && !item.isActivationLabel);
                            p.bindPromptLabelInteraction(el, item);
                            el.style.backgroundColor = item.eyeToggled ? p.BLUE : '';
                            el.style.color = item.eyeToggled ? p.WHITE : '';
            }

            p.applyHighlight();
          };
        })();");
        }

        private void InjectPromptOverlayPart4()
        {
            Eval(
                @"(() => {
          const p = window.overjoyedParts;
          if (!p) return;

          p.applyEyeLabelVisualState = el => {
                if (!el || !el.classList.contains('overjoyed-eye-label')) {
                  return;
                }

                el.style.backgroundColor = p.state.fpsDeadZoneLookModeLatched ? p.BLUE : '';
                el.style.color = p.state.fpsDeadZoneLookModeLatched ? p.WHITE : '';
          };

                  p.shouldBypassRegularHighlightForEye = el =>
                    !!el &&
                    el.classList.contains('overjoyed-eye-label') &&
                    !!p.state.showLabelsLC &&
                    !!p.state.chkFPS;

                  p.shouldSuppressEyeQuadrant = quadrant =>
                    quadrant === 27 && !!p.state.showLabelsLC && !!p.state.chkFPS;

          p.clearHighlight = () => {
                        p.getAllPromptElements().forEach(el => {
              el.classList.remove('overjoyed-highlighted');
                            el.style.backgroundImage = '';
                            el.style.backgroundColor = '';
                            el.style.color = '';

                            const fill = el.querySelector('.prompt-fill');
                            if (fill) {
                                fill.style.backgroundImage = '';
                            }

                            p.applyEyeLabelVisualState(el);
            });
          };

                    p.isTransparentExitHoldLabel = el =>
                        !!el && el.classList.contains('exit-hold-label') && el.classList.contains('exit-hold-lower');

                    p.getExtraButtonsBottom = () => {
                        const extraButtons =
                            document.querySelector('div[name=""pnlExtraButtons""]') ||
                            document.querySelector('[name=""pnlExtraButtons""]') ||
                            document.querySelector('[id$=""pnlExtraButtons""]') ||
                            document.getElementById('pnlExtraButtons');

                        if (!extraButtons) return null;

                        const style = window.getComputedStyle(extraButtons);
                        if (style.display === 'none' || style.visibility === 'hidden') return null;

                        const rect = extraButtons.getBoundingClientRect();
                        if (rect.height <= 0) return null;

                        return rect.bottom;
                    };

                    p.shouldHidePointer = el => {
                        if (!el) return false;

                        const bottom = p.getExtraButtonsBottom();
                        if (bottom == null) return false;

                        const rect = el.getBoundingClientRect();
                        const centerY = rect.top + (rect.height / 2);
                        return centerY <= bottom;
                    };

                    p.updateOverlayVisibility = () => {
                        const cursor = p.byId('cursorSquare');
                        const socket = p.byId('socketSquare');
                        const remoteMode = p.isRemoteCheckboxChecked();
                        const active = p.canShowHighlights();

                        p.syncPointerElements();

                        p.getActivationLabelElements().forEach(el => {
                            el.style.visibility = active ? 'visible' : 'hidden';
                        });

                        if (cursor) {
                            cursor.style.visibility = (!remoteMode && !p.shouldHidePointer(cursor)) ? 'visible' : 'hidden';
                        }

                        if (socket) {
                            socket.style.visibility = remoteMode ? 'visible' : 'hidden';
                        }
                    };

          p.paintHoverElement = el => {
                        if (el.classList.contains('exit-hold-label')) {
                                                        el.style.backgroundColor = p.isTransparentExitHoldLabel(el) ? 'transparent' : p.BLACK;
                            el.style.color = p.WHITE;
                            return;
                        }

                  if (p.shouldBypassRegularHighlightForEye(el)) {
                    el.classList.remove('overjoyed-highlighted');
                    el.style.backgroundImage = '';
                    p.applyEyeLabelVisualState(el);
                    return;
                  }

            el.classList.add('overjoyed-highlighted');
          };

                    p.getHoverFillDirection = quadrant => {
                        switch (quadrant) {
                            case 0: return 'to top';
                            case 1: return 'to top right';
                            case 2: return 'to right';
                            case 3: return 'to bottom right';
                            case 4: return 'to bottom';
                            case 5: return 'to bottom left';
                            case 6: return 'to left';
                            case 7: return 'to top left';
                            case 24: return 'to top';
                            case 27: return 'to top';
                            case 30: return 'to bottom';
                            default: return 'to top';
                        }
                    };

                                        p.paintHoverFillElement = (el, ratio, quadrant) => {
                        const clampedRatio = Math.max(0, Math.min(1, Number(ratio) || 0));
                        const percent = `${clampedRatio * 100}%`;
                                                const direction = p.getHoverFillDirection(quadrant);
                                                const gradient = `linear-gradient(${direction}, ${p.BLUE} 0 ${percent}, transparent ${percent} 100%)`;

                        if (el.classList.contains('prompt-box')) {
                            const fill = el.querySelector('.prompt-fill');
                            if (fill) {
                                                                fill.style.backgroundImage = gradient;
                            }
                            return;
                        }

                        if (p.shouldBypassRegularHighlightForEye(el)) {
                          el.style.backgroundImage = '';
                          p.applyEyeLabelVisualState(el);
                          return;
                        }

                                                el.style.backgroundImage = gradient;
                        el.style.backgroundColor = 'transparent';
                        el.style.color = p.WHITE;
                    };

          p.getHoverTargets = quadrant => {
            switch (quadrant) {
              case 0: return { boxes: [0], labels: [] };
                            case 1: return p.isElementVisible(p.getPromptBoxElement(1)) ? { boxes: [1], labels: [] } : { boxes: [10, 11], labels: [] };
              case 2: return { boxes: [2], labels: [] };
                            case 3: return p.isElementVisible(p.getPromptBoxElement(3)) ? { boxes: [3], labels: [] } : { boxes: [14, 15], labels: [] };
              case 4: return { boxes: [4], labels: [] };
                            case 5: return p.isElementVisible(p.getPromptBoxElement(5)) ? { boxes: [5], labels: [] } : { boxes: [12, 13], labels: [] };
              case 6: return { boxes: [6], labels: [] };
                            case 7: return p.isElementVisible(p.getPromptBoxElement(7)) ? { boxes: [7], labels: [] } : { boxes: [8, 9], labels: [] };
              case 24: return { boxes: [], labels: [0] };
              case 27: return p.state.showLabelsLC && p.state.chkFPS
                ? { boxes: [], labels: [] }
                : { boxes: [], labels: [1] };
              case 30: return { boxes: [], labels: [2] };
              default: return { boxes: [], labels: [] };
            }
          };

          p.getQuadrantConfigIndex = quadrant => {
            if (quadrant == null) return null;

            if (p.state.showLabelsHover) {
              if (quadrant >= 0 && quadrant <= 7) return quadrant;
              if (quadrant === 24 || quadrant === 27 || quadrant === 30) return quadrant;
              return null;
            }

            if (p.state.showLabelsLC) {
              if (quadrant >= 0 && quadrant <= 7) return quadrant + 8;
              if (quadrant === 24) return 25;
              if (quadrant === 27) return 28;
              if (quadrant === 30) return 31;
              return null;
            }

            if (p.state.showLabelsRC) {
              if (quadrant >= 0 && quadrant <= 7) return quadrant + 16;
              if (quadrant === 24) return 26;
              if (quadrant === 27) return 29;
              if (quadrant === 30) return 32;
              return null;
            }

            return null;
          };

                    p.getQuadrantPromptConfigIndices = quadrant => {
                        const configIndex = p.getQuadrantConfigIndex(quadrant);
                        if (configIndex == null) return [];

                        if (p.state.showLabelsHover && [1, 3, 5, 7].includes(quadrant) && p.isDiagonalEnabled(quadrant)) {
                            const base = p.getActivePromptBaseIndex();
                            const prevIndex = base + quadrant - 1;
                            const nextIndex = base + ((quadrant + 1) % 8);
                            const merged = p.tryGetMergedPromptPath(quadrant, prevIndex, nextIndex);
                            if (merged) {
                                return [prevIndex, nextIndex];
                            }
                        }

                        return [configIndex];
                    };

                    p.isVariableFillModeValue = modeValue =>
                        modeValue.includes('ls ') ||
                        modeValue.includes('rs ') ||
                        modeValue.includes('lt ') ||
                        modeValue.includes('rt ');

          p.isToggleQuadrant = quadrant => {
            const configIndex = p.getQuadrantConfigIndex(quadrant);
            return configIndex != null &&
              String(p.state.buttonConfig[configIndex]?.[1] || '').toLowerCase() === 'true';
          };

          p.isOnceQuadrant = quadrant => {
            const configIndex = p.getQuadrantConfigIndex(quadrant);
            return configIndex != null &&
              String(p.state.buttonConfig[configIndex]?.[1] || '').toLowerCase() === 'once';
          };

                    p.isHoverFillQuadrant = quadrant => {
                        if (!p.state.showLabelsHover) return false;
                        if (!(p.state.xboxMode || p.state.switchMode)) return false;

                        const promptConfigIndices = p.getQuadrantPromptConfigIndices(quadrant);
                        if (!promptConfigIndices.length) return false;

                        return promptConfigIndices.some(configIndex => {
                            if (String(p.state.buttonConfig[configIndex]?.[5] || '').toLowerCase() !== 'true') return false;

                            const modeValue = p.state.xboxMode
                                ? String(p.state.buttonConfig[configIndex]?.[3] || '').toLowerCase()
                                : String(p.state.buttonConfig[configIndex]?.[6] || '').toLowerCase();

                            return p.isVariableFillModeValue(modeValue);
                        });
                    };

                    p.getQuadrantInteractionType = quadrant => {
                        if (quadrant == null) return null;
                        if (p.isToggleQuadrant(quadrant)) return 'toggle';
                        if (p.isOnceQuadrant(quadrant)) return 'once';
                        return 'regular';
                    };

          p.isLeftClickHeld = () => !!p.state.lcPointerDown;

          p.hasLcTransitionState = () => !!p.state.lcTransitionPending;

          p.setLcTransitionState = enabled => {
            p.state.lcTransitionPending = !!enabled;
          };

          p.isLeftClickPromptHighlightActive = () =>
            p.state.showLabelsLC && p.state.lcPointerDown && !p.state.lcTransitionPending;

          p.isRightClickPromptHighlightActive = () =>
            p.state.showLabelsRC && !!p.state.rcPointerDown;

          p.canShowHighlights = () => {
                        const activeLabel = p.getLblActive();
                        if (!activeLabel) return false;

                        const state = activeLabel.dataset.overjoyedState || p.syncLblActiveStateFromDom();
                        return state === 'active';
          };

          p.getOrCreateLcCycle = quadrant => {
            if (!p.state.lcToggleCycles[quadrant]) {
              p.state.lcToggleCycles[quadrant] = { isOn: false, wasActive: false, readyForActive: false };
            }
            return p.state.lcToggleCycles[quadrant];
          };

          p.getOrCreateRcCycle = quadrant => {
            if (!p.state.rcToggleCycles[quadrant]) {
              p.state.rcToggleCycles[quadrant] = { isOn: false, wasActive: false, readyForActive: false };
            }
            return p.state.rcToggleCycles[quadrant];
          };

          p.canUseLcTrigger = quadrant =>
            quadrant != null && !!p.state.lcToggleCycles[quadrant]?.readyForActive;

          p.canUseRcTrigger = quadrant =>
            quadrant != null && !!p.state.rcToggleCycles[quadrant]?.readyForActive;

                    p.paintExitHoldProgress = () => {
                        const ratio = Math.max(0, Math.min(1, Number(p.state.exitHoldRatio) || 0));
                        p.paintActivationZoneProgress(ratio);
                    };

                    p.clearExitHoldProgress = () => {
                        if (p.intervalId) {
                            clearInterval(p.intervalId);
                            p.intervalId = null;
                        }

                        p.state.exitHoldStartedAt = 0;
                        p.state.exitHoldRatio = 0;
                        p.state.exitHoldTriggered = false;
                        p.paintExitHoldProgress();
                    };

                    p.isExitHoldActive = () => {
                        const activationQuadrant = p.getActivationDeadZoneQuadrant();
                        if (activationQuadrant == null) return false;
                        if (p.state.activeHoverQuadrant !== activationQuadrant) return false;

                        const leftHolding = p.state.lcPointerDown && p.state.lcPressQuadrant === activationQuadrant;
                        const rightHolding = p.state.rcPointerDown && p.state.rcPressQuadrant === activationQuadrant;
                        return leftHolding || rightHolding;
                    };

                    p.updateExitHoldProgress = () => {
                        if (!p.isExitHoldActive()) {
                            p.clearExitHoldProgress();
                            return;
                        }

                        if (!p.state.exitHoldStartedAt) {
                            p.state.exitHoldStartedAt = Date.now();
                        }

                        const elapsed = Date.now() - p.state.exitHoldStartedAt;
                        p.state.exitHoldRatio = Math.max(0, Math.min(1, elapsed / Math.max(1, p.state.exitHoldDurationMs || 3000)));
                        p.paintExitHoldProgress();

                        if (p.state.exitHoldRatio >= 1 && !p.state.exitHoldTriggered) {
                            p.state.exitHoldTriggered = true;

                            if (p.intervalId) {
                                clearInterval(p.intervalId);
                                p.intervalId = null;
                            }

                            if (p.host && typeof p.host.ExitHoldDeactivate === 'function') {
                                p.host.ExitHoldDeactivate();
                            }

                            return;
                        }

                        if (!p.intervalId) {
                            p.intervalId = setInterval(() => {
                                if (!p.isExitHoldActive()) {
                                    p.clearExitHoldProgress();
                                    return;
                                }

                                const runningElapsed = Date.now() - p.state.exitHoldStartedAt;
                                p.state.exitHoldRatio = Math.max(0, Math.min(1, runningElapsed / Math.max(1, p.state.exitHoldDurationMs || 3000)));
                                p.paintExitHoldProgress();

                                if (p.state.exitHoldRatio >= 1 && !p.state.exitHoldTriggered) {
                                    p.state.exitHoldTriggered = true;

                                    if (p.intervalId) {
                                        clearInterval(p.intervalId);
                                        p.intervalId = null;
                                    }

                                    if (p.host && typeof p.host.ExitHoldDeactivate === 'function') {
                                        p.host.ExitHoldDeactivate();
                                    }
                                }
                            }, 40);
                        }
                    };

          p.applyHighlight = () => {
            p.clearHighlight();

                        const finish = () => {
                            p.updateExitHoldProgress();
                        };

            if (!p.canShowHighlights()) {
                            finish();
                            return;
            }

            Object.keys(p.state.highlightToggleStates).forEach(key => {
              const quadrant = Number(key);
              if (!p.state.highlightToggleStates[key]) return;
              if (p.shouldSuppressEyeQuadrant(quadrant)) return;

              const targets = p.getHoverTargets(quadrant);

              targets.boxes.forEach(slot => {
                const el = p.layer.querySelector(`.prompt-box[data-slot='${slot}']`);
                if (el) p.paintHoverElement(el);
              });

              targets.labels.forEach(slot => {
                                const el = p.getPromptLabelElement(slot);
                if (el) p.paintHoverElement(el);
              });
            });

            if (p.state.showLabelsLC && p.state.heldLcQuadrant != null) {
              if (p.shouldSuppressEyeQuadrant(p.state.heldLcQuadrant)) {
                            finish();
                            return;
              }

              const targets = p.getHoverTargets(p.state.heldLcQuadrant);

              targets.boxes.forEach(slot => {
                const el = p.layer.querySelector(`.prompt-box[data-slot='${slot}']`);
                if (el) p.paintHoverElement(el);
              });

              targets.labels.forEach(slot => {
                                const el = p.getPromptLabelElement(slot);
                if (el) p.paintHoverElement(el);
              });

                            finish();
                            return;
            }

            if (p.state.showLabelsHover) {
              const quadrant = p.state.activeHoverQuadrant;
              if (quadrant == null) return;
              if (p.shouldSuppressEyeQuadrant(quadrant)) {
                            finish();
                            return;
              }
              if (p.isToggleQuadrant(quadrant) || p.isOnceQuadrant(quadrant)) return;

              const targets = p.getHoverTargets(quadrant);
                            const hasMultiplePromptTargets = (targets.boxes.length + targets.labels.length) > 1;
                            const useHoverFill = p.isHoverFillQuadrant(quadrant) && !hasMultiplePromptTargets;
                            const hoverFillRatio = Math.max(0, Math.min(1, Number(p.state.hoverFillRatio) || 0));

              targets.boxes.forEach(slot => {
                const el = p.layer.querySelector(`.prompt-box[data-slot='${slot}']`);
                                if (!el) return;
                                if (useHoverFill) p.paintHoverFillElement(el, hoverFillRatio, quadrant);
                                else p.paintHoverElement(el);
              });
              targets.labels.forEach(slot => {
                                const el = p.getPromptLabelElement(slot);
                                if (!el) return;
                                if (useHoverFill) p.paintHoverFillElement(el, hoverFillRatio, quadrant);
                                else p.paintHoverElement(el);
              });
                            finish();
                            return;
            }

            const quadrant = p.state.activeHoverQuadrant;
                        if (quadrant == null) {
                            finish();
                            return;
                        }
                  if (p.shouldSuppressEyeQuadrant(quadrant)) {
                    finish();
                    return;
                  }
                        if (p.isToggleQuadrant(quadrant) || p.isOnceQuadrant(quadrant)) {
                            finish();
                            return;
                        }

                        if (p.state.showLabelsLC && p.state.lcPointerDown && p.state.lcPressQuadrant != null && quadrant !== p.state.lcPressQuadrant) {
                            finish();
                            return;
                        }
                        if (p.state.showLabelsRC && p.state.rcPointerDown && p.state.rcPressQuadrant != null && quadrant !== p.state.rcPressQuadrant) {
                            finish();
                            return;
                        }

            const lcActive = p.isLeftClickPromptHighlightActive();
                        if (p.state.showLabelsLC && (!lcActive || !p.canUseLcTrigger(quadrant))) {
                            finish();
                            return;
                        }

            const rcActive = p.isRightClickPromptHighlightActive();
                        if (p.state.showLabelsRC && (!rcActive || !p.canUseRcTrigger(quadrant))) {
                            finish();
                            return;
                        }

            const targets = p.getHoverTargets(quadrant);

            targets.boxes.forEach(slot => {
              const el = p.layer.querySelector(`.prompt-box[data-slot='${slot}']`);
              if (el) p.paintHoverElement(el);
            });

            targets.labels.forEach(slot => {
                            const el = p.getPromptLabelElement(slot);
              if (el) p.paintHoverElement(el);
            });

                        finish();
          };

          p.syncInteractiveQuadrantState = () => {
            const lcActive = p.isLeftClickPromptHighlightActive();
            const rcActive = p.isRightClickPromptHighlightActive();
            const quadrant = p.state.activeHoverQuadrant;

            if (p.state.showLabelsLC) {
              const lcHeld = p.isLeftClickHeld();
              if (lcHeld) {
                p.cancelLcHeldRelease();
              }

                            if (!lcHeld) {
                p.setLcTransitionState(false);
                p.state.lastEnteredHoverQuadrant = null;
                p.state.lcToggleCycles = Object.create(null);

                if (p.state.heldLcQuadrant != null) {
                                    const elapsed = Date.now() - (p.state.lcHoldStartedAt || 0);
                                    const remaining = Math.max(0, (p.state.lcMinHighlightMs || 250) - elapsed);

                  p.cancelLcHeldRelease();

                                    if (remaining > 0) {
                                        p.state.lcHeldReleaseTimeout = setTimeout(() => {
                                            p.clearLcHeldHighlight();
                                            p.state.lcHeldReleaseTimeout = null;
                                            p.applyHighlight();
                                        }, remaining);

                                        p.applyHighlight();
                                        return;
                                    }

                                    p.clearLcHeldHighlight();
                }

                p.applyHighlight();
                return;
              }

                            if (p.state.lcPressType === 'regular' && p.state.heldLcQuadrant != null) {
                                p.setLcTransitionState(false);
                                p.applyHighlight();
                                return;
                            }

              if (quadrant == null) {
                p.setLcTransitionState(false);

                if (!lcActive && p.state.heldLcQuadrant != null) {
                  p.clearLcHeldHighlight();
                }

                p.state.lastEnteredHoverQuadrant = null;
                p.applyHighlight();
                return;
              }

              const isToggle = p.isToggleQuadrant(quadrant);
              const isOnce = p.isOnceQuadrant(quadrant);
              const isRegular = !isToggle && !isOnce;
              const pressStartedOnRegular = !!p.state.lcRegularHoldAllowed;
                            const samePressQuadrant =
                                p.state.lcPressQuadrant == null ||
                                quadrant === p.state.lcPressQuadrant;

              if (isRegular && p.state.heldLcQuadrant != null) {
                if (lcActive) {
                  p.applyHighlight();
                  return;
                }

                p.clearLcHeldHighlight();

                const resetCycle = p.getOrCreateLcCycle(quadrant);
                resetCycle.wasActive = false;
                resetCycle.readyForActive = true;
                p.state.lastEnteredHoverQuadrant = quadrant;
                p.applyHighlight();
                return;
              }

              const cycle = p.getOrCreateLcCycle(quadrant);

              if (isToggle) {
                cycle.isOn = !!p.state.highlightToggleStates[quadrant];
              }

              const quadrantChanged = quadrant !== p.state.lastEnteredHoverQuadrant;

              if (quadrantChanged) {
                p.setLcTransitionState(true);
                p.state.lastEnteredHoverQuadrant = quadrant;
                cycle.wasActive = false;
                                cycle.readyForActive = samePressQuadrant && (!isRegular || !lcActive || pressStartedOnRegular);
                p.applyHighlight();
                return;
              }

              if (p.hasLcTransitionState()) {
                p.setLcTransitionState(false);
              }

              if (!lcActive) {
                cycle.wasActive = false;
                                cycle.readyForActive = samePressQuadrant && (!isRegular || pressStartedOnRegular);

                if (p.state.heldLcQuadrant != null) {
                  p.clearLcHeldHighlight();
                }

                p.applyHighlight();
                return;
              }

              const canUseCurrentLcPress =
                quadrant != null &&
                quadrant === p.state.lcPressQuadrant &&
                !!p.state.lcPressTriggerArmed;

                    const activeEdge = !cycle.wasActive && cycle.readyForActive;

                    if (activeEdge) {
                        if (isToggle || isOnce) {
                            if (!canUseCurrentLcPress) {
                                cycle.readyForActive = false;
                                cycle.wasActive = true;
                                p.applyHighlight();
                                return;
                            }
                        }

                        if (isToggle) {
                            cycle.isOn = !cycle.isOn;
                            p.state.highlightToggleStates[quadrant] = cycle.isOn;
                        } else if (isOnce) {
                            p.state.highlightToggleStates[quadrant] = true;

                            if (p.state.highlightOnceTimeouts[quadrant]) {
                                clearTimeout(p.state.highlightOnceTimeouts[quadrant]);
                            }

                            p.state.highlightOnceTimeouts[quadrant] = setTimeout(() => {
                                p.state.highlightToggleStates[quadrant] = false;
                                delete p.state.highlightOnceTimeouts[quadrant];
                                p.applyHighlight();
                            }, 500);
                        } else {
                            p.cancelLcHeldRelease();
                            p.state.heldLcQuadrant = quadrant;
                            p.state.lcHoldStartedAt = Date.now();
                        }

                        p.state.lcPressTriggerArmed = false;
                        cycle.readyForActive = false;
                    }

                    cycle.wasActive = true;
                    p.applyHighlight();
                    return;
            }

            if (p.state.showLabelsRC) {
              if (quadrant == null) {
                p.state.lastEnteredHoverQuadrant = null;
                p.applyHighlight();
                return;
              }

              const cycle = p.getOrCreateRcCycle(quadrant);
                            const samePressQuadrant =
                                p.state.rcPressQuadrant == null ||
                                quadrant === p.state.rcPressQuadrant;

              if (quadrant !== p.state.lastEnteredHoverQuadrant) {
                cycle.wasActive = rcActive;
                                cycle.readyForActive = samePressQuadrant && !rcActive;
                p.state.lastEnteredHoverQuadrant = quadrant;
                p.applyHighlight();
                return;
              }

              if (!rcActive) {
                                cycle.readyForActive = samePressQuadrant;
              }

              const triggerEdge = rcActive && !cycle.wasActive && cycle.readyForActive;

              if (triggerEdge) {
                if (p.isToggleQuadrant(quadrant)) {
                  cycle.isOn = !cycle.isOn;
                  p.state.highlightToggleStates[quadrant] = cycle.isOn;
                } else if (p.isOnceQuadrant(quadrant)) {
                  p.state.highlightToggleStates[quadrant] = true;

                  if (p.state.highlightOnceTimeouts[quadrant]) {
                    clearTimeout(p.state.highlightOnceTimeouts[quadrant]);
                  }

                  p.state.highlightOnceTimeouts[quadrant] = setTimeout(() => {
                    p.state.highlightToggleStates[quadrant] = false;
                    delete p.state.highlightOnceTimeouts[quadrant];
                    p.applyHighlight();
                  }, 500);
                }
              }

              cycle.wasActive = rcActive;
              p.applyHighlight();
              return;
            }

            if (!(p.state.showLabelsHover || (p.state.showLabelsLC && p.isLeftClickPromptHighlightActive()))) {
              p.state.lastEnteredHoverQuadrant = null;
              p.applyHighlight();
              return;
            }

            if (quadrant == null) {
              p.applyHighlight();
              return;
            }

            if (quadrant !== p.state.lastEnteredHoverQuadrant) {
              if (p.isToggleQuadrant(quadrant)) {
                p.state.highlightToggleStates[quadrant] = !p.state.highlightToggleStates[quadrant];
              } else if (p.isOnceQuadrant(quadrant)) {
                p.state.highlightToggleStates[quadrant] = true;

                if (p.state.highlightOnceTimeouts[quadrant]) {
                  clearTimeout(p.state.highlightOnceTimeouts[quadrant]);
                }

                p.state.highlightOnceTimeouts[quadrant] = setTimeout(() => {
                  p.state.highlightToggleStates[quadrant] = false;
                  delete p.state.highlightOnceTimeouts[quadrant];
                  p.applyHighlight();
                }, 500);
              }
            }

            p.state.lastEnteredHoverQuadrant = quadrant;
            p.applyHighlight();
          };
        })();");
        }

        private void InjectPromptOverlayPart5()
        {
            Debug.WriteLine("[InjectPromptOverlayPart5] Injecting overlay runtime hooks.");

            Eval(
                @"(() => {
          const p = window.overjoyedParts;
          if (!p) return;

                    p.isPart5DebugEnabled = () =>
                        window.overjoyedPromptDebugEnabled === true ||
                        window.overjoyedPromptDebugPart5Enabled === true;

                    p.shouldDebugPart5 = (key, throttleMs) => {
                        if (!p.isPart5DebugEnabled()) {
                            return false;
                        }

                        if (!throttleMs || throttleMs <= 0) {
                            return true;
                        }

                        const now = Date.now();
                        const lastByKey = p.part5DebugLastByKey = p.part5DebugLastByKey || Object.create(null);
                        const last = lastByKey[key] || 0;
                        if ((now - last) < throttleMs) {
                            return false;
                        }

                        lastByKey[key] = now;
                        return true;
                    };

                    p.debugPart5 = (eventName, details) => {
                        if (!p.isPart5DebugEnabled()) {
                            return;
                        }

                        if (typeof p.debugPart2 === 'function') {
                            p.debugPart2(`part5:${eventName}`, details);
                            return;
                        }

                        try {
                            const store = window.overjoyedPromptDebugLog = window.overjoyedPromptDebugLog || [];
                            const entry = {
                                timestamp: new Date().toISOString(),
                                eventName: `part5:${eventName}`,
                                details: details || null
                            };

                            store.push(entry);
                            if (store.length > 200) {
                                store.splice(0, store.length - 200);
                            }

                            console.log('[Overjoyed][PromptOverlayPart5]', eventName, details || '');
                        } catch (error) {
                            console.warn('[Overjoyed][PromptOverlayPart5][debug-failed]', error);
                        }
                    };

                    window.overjoyedPromptDebugPart5 = p.debugPart5;
                    window.setOverjoyedPromptDebugPart5 = enabled => {
                        window.overjoyedPromptDebugPart5Enabled = !!enabled;
                        return window.overjoyedPromptDebugPart5Enabled;
                    };
                    p.lastDebugSectorKey = null;
                    p.lastDebugUpdateSource = null;

                    p.debugSectorState = (stateKey, details) => {
                        if (p.lastDebugSectorKey === stateKey) {
                            return;
                        }

                        p.lastDebugSectorKey = stateKey;
                        p.debugPart5('sector', Object.assign({ stateKey }, details || {}));
                    };

                    p.debugPart5('init', {
                        hasContainer: !!p.container,
                        hasLayer: !!p.layer,
                        x: p.x,
                        y: p.y,
                        showLabelsHover: !!p.state?.showLabelsHover,
                        showLabelsLC: !!p.state?.showLabelsLC,
                        showLabelsRC: !!p.state?.showLabelsRC,
                        activeHoverQuadrant: p.state?.activeHoverQuadrant ?? null
                    });

          p.resetSectorVisuals = () => {
            for (let i = 1; i <= 8; i++) {
              const line = p.byId(`line${i}`);
              if (line) line.style.stroke = p.WHITE;
            }

            const upperActive = p.byId('upperDZ-active');
            const lowerZone = p.byId('lowerDZ');
            const upperInactive = p.byId('upperDZ-inactive');
            const middleOuter = p.byId('middleDZ-outer');
            const middleInner = p.byId('middleDZ-inner');
            const middleBorder = p.byId('middleDZ-border');

            if (upperActive) upperActive.style.stroke = 'none';
            if (lowerZone) lowerZone.style.stroke = p.WHITE;
            if (upperInactive) upperInactive.style.stroke = p.WHITE;
            if (middleOuter) middleOuter.style.stroke = 'none';
            if (middleInner) middleInner.style.stroke = 'none';
            if (middleBorder) middleBorder.style.stroke = 'none';

            p.applyDefaultFills();
          };

          p.setHoverQuadrant = quadrant => {
            p.state.activeHoverQuadrant = quadrant;
            p.syncInteractiveQuadrantState();
          };

          p.getCenter = () => {
            const rect = p.container.getBoundingClientRect();
            return { cx: rect.width / 2, cy: rect.height * (350 / 600) };
          };

                    p.isSafeZoneVisible = () => {
                        const safeZone = p.byId('safeZone');
                        if (!safeZone) return false;

                        const style = window.getComputedStyle(safeZone);
                        return style.visibility !== 'hidden' && style.display !== 'none' && style.opacity !== '0';
                    };

          p.updateSector = (px, py) => {
            const { cx, cy } = p.getCenter();
                        const rect = p.container.getBoundingClientRect();
            const dx = px - cx;
            const dy = py - cy;
            const distance = Math.sqrt(dx * dx + dy * dy);
            const deadZone = 60;
                        const radius = rect.height * 0.3;
                        const safeZonePadding = p.isSafeZoneVisible() ? 38 : 0;
                        const hoverActivationDistance = deadZone + safeZonePadding;

                        p.state.hoverFillRatio = 0;

            p.x = px;
            p.y = py;

                        p.syncPointerElements();

            p.resetSectorVisuals();

            if (!p.canShowHighlights()) {
                            p.debugSectorState('inactive', {
                                px,
                                py,
                                distance: Math.round(distance),
                                canShowHighlights: false
                            });
              p.setHoverQuadrant(null);
                            p.updateOverlayVisibility();
              return;
            }

            if (distance <= deadZone / 2) {
                            p.state.hoverFillRatio = Math.max(0, Math.min(1, distance / (deadZone / 2)));
                            p.debugSectorState('middle', {
                                px,
                                py,
                                distance: Math.round(distance),
                                hoverFillRatio: p.state.hoverFillRatio
                            });
              p.setHoverQuadrant(27);

              const middleOuter = p.byId('middleDZ-outer');
              const middleInner = p.byId('middleDZ-inner');
              const middleBorder = p.byId('middleDZ-border');

              if (middleOuter) middleOuter.style.stroke = p.WHITE;
              if (middleInner) middleInner.style.stroke = p.BLUE;
              if (middleBorder) middleBorder.style.stroke = p.WHITE;
                            p.updateOverlayVisibility();
              return;
            }

            if (distance <= deadZone) {
                            p.state.hoverFillRatio = Math.max(0, Math.min(1, (distance - deadZone / 2) / (deadZone / 2)));
                            p.debugSectorState(dy < 0 ? 'upper-deadzone' : 'lower-deadzone', {
                                px,
                                py,
                                distance: Math.round(distance),
                                hoverFillRatio: p.state.hoverFillRatio,
                                dy: Math.round(dy)
                            });
              p.setHoverQuadrant(dy < 0 ? 24 : 30);

              const upperActive = p.byId('upperDZ-active');
              const lowerZone = p.byId('lowerDZ');
              const upperInactive = p.byId('upperDZ-inactive');

              if (dy < 0) {
                if (upperActive) upperActive.style.stroke = p.BLUE;
                if (lowerZone) lowerZone.style.stroke = p.WHITE;
                if (upperInactive) upperInactive.style.stroke = 'none';
              } else {
                if (upperActive) upperActive.style.stroke = 'none';
                if (lowerZone) lowerZone.style.stroke = p.BLUE;
                if (upperInactive) upperInactive.style.stroke = p.WHITE;
              }

                            p.updateOverlayVisibility();
              return;
            }

                        if (distance <= hoverActivationDistance) {
                            p.debugSectorState('safe-buffer', {
                                px,
                                py,
                                distance: Math.round(distance),
                                hoverActivationDistance,
                                safeZoneVisible: p.isSafeZoneVisible()
                            });
                            p.setHoverQuadrant(null);
                                                        p.updateOverlayVisibility();
                            return;
                        }

            let angle = Math.atan2(dy, dx) * 180 / Math.PI;

            if (Math.abs(angle + 112.5) < 1.0 || Math.abs(angle + 67.5) < 1.0) angle = -90;
            else if (Math.abs(angle + 22.5) < 1.0 || Math.abs(angle - 22.5) < 1.0) angle = 0;
            else if (Math.abs(angle - 67.5) < 1.0 || Math.abs(angle - 112.5) < 1.0) angle = 90;
            else if (Math.abs(angle + 157.5) < 1.0 || Math.abs(angle - 157.5) < 1.0 || Math.abs(Math.abs(angle) - 180) < 1.0) angle = 179.999;

            function light(a, b, quadrant) {
              const first = p.byId(a);
              const second = p.byId(b);
              if (first) first.style.stroke = p.BLUE;
              if (second) second.style.stroke = p.BLUE;
                                                        p.state.hoverFillRatio = Math.max(0, Math.min(1, (distance - hoverActivationDistance) / Math.max(1, radius - hoverActivationDistance)));
                            p.debugSectorState(`quadrant-${quadrant}`, {
                                px,
                                py,
                                quadrant,
                                angle: Math.round(angle * 10) / 10,
                                distance: Math.round(distance),
                                hoverFillRatio: p.state.hoverFillRatio
                            });
              p.setHoverQuadrant(quadrant);
            }

            if (angle >= -112.5 && angle < -67.5) light('line1', 'line2', 0);
            else if (angle >= -67.5 && angle < -22.5) light('line2', 'line3', 1);
            else if (angle >= -22.5 && angle < 22.5) light('line3', 'line4', 2);
            else if (angle >= 22.5 && angle < 67.5) light('line4', 'line5', 3);
            else if (angle >= 67.5 && angle < 112.5) light('line5', 'line6', 4);
            else if (angle >= 112.5 && angle < 157.5) light('line6', 'line7', 5);
            else if (angle >= 157.5 || angle < -157.5) light('line7', 'line8', 6);
            else if (angle >= -157.5 && angle < -112.5) light('line8', 'line1', 7);
            else p.setHoverQuadrant(null);

                        p.updateOverlayVisibility();
          };

          p.updateFromState = () => {
          if(p.isRemoteCheckboxChecked()) {
                        if (p.lastDebugUpdateSource !== 'remote') {
                            p.lastDebugUpdateSource = 'remote';
                            p.debugPart5('updateFromState', {
                                source: 'remote',
                                remoteX: p.state.remoteX,
                                remoteY: p.state.remoteY
                            });
                        }
                        p.syncPointerElements();
                        const remotePointer = document.getElementById('socketSquare');
                        if (!remotePointer) return;

                        const remoteHalf = p.getPointerHalfSize(remotePointer);
                        const remoteX = (parseFloat(remotePointer.style.left) || 0) + remoteHalf;
                        const remoteY = (parseFloat(remotePointer.style.top) || 0) + remoteHalf;
                            p.updateSector(remoteX, remoteY);
                            return;
            }

                        if (p.lastDebugUpdateSource !== 'local') {
                            p.lastDebugUpdateSource = 'local';
                            p.debugPart5('updateFromState', {
                                source: 'local',
                                x: p.x,
                                y: p.y
                            });
                        }

            p.updateSector(p.x, p.y);
          };

          p.centerPointer = () => {
            const rect = p.container.getBoundingClientRect();
            p.x = rect.width / 2;
            p.y = rect.height * (350 / 600);
            p.updateSector(p.x, p.y);
          };

          p.clearAllHighlightsImmediately = () => {
            p.state.activeHoverQuadrant = null;
            p.state.lastEnteredHoverQuadrant = null;
            p.state.lcPointerDown = false;
            p.state.rcPointerDown = false;
            p.state.lcTransitionPending = false;
            p.cancelLcHeldRelease();
                        p.clearExitHoldProgress();
            p.clearLcHeldHighlight();
            p.clearHighlight();
            p.resetSectorVisuals();
                        p.updateOverlayVisibility();
          };

          p.windowPrompts = {
            state: p.state,
            clear: p.clearPromptLayer,
            render: p.renderPrompts,
            setHoverQuadrant(quadrant) {
              p.state.activeHoverQuadrant = quadrant;
              p.syncInteractiveQuadrantState();
            },
            setState(nextState) {
              Object.assign(p.state, nextState || {});

                                                        if (p.shouldDebugPart5('setState', 250)) {
                            p.debugPart5('setState', {
                                keys: Object.keys(nextState || {}),
                                showLabelsHover: !!p.state.showLabelsHover,
                                showLabelsLC: !!p.state.showLabelsLC,
                                showLabelsRC: !!p.state.showLabelsRC,
                                activeHoverQuadrant: p.state.activeHoverQuadrant ?? null
                            });
                                                        }

              if (!p.state.showLabelsHover && !p.state.showLabelsLC && !p.state.showLabelsRC) {
                Object.keys(p.state.highlightOnceTimeouts).forEach(key => {
                  clearTimeout(p.state.highlightOnceTimeouts[key]);
                });

                p.clearLcHeldHighlight();
            p.cancelLcHeldRelease();
                p.state.lcPointerDown = false;
                p.state.rcPointerDown = false;
                p.state.lcTransitionPending = false;
                p.state.activeHoverQuadrant = null;
                p.state.lastEnteredHoverQuadrant = null;
                p.state.highlightToggleStates = Object.create(null);
                p.state.highlightOnceTimeouts = Object.create(null);
                p.state.lcToggleCycles = Object.create(null);
                p.state.rcToggleCycles = Object.create(null);
              }

              p.renderPrompts();
              p.updateFromState();
            }
          };

          window.overjoyedPrompts = p.windowPrompts;

                                        const activeLabel = p.getLblActive();
                    if (activeLabel) {
                                                p.attachLblActiveEventHandlers();
                                                p.syncLblActiveStateFromDom();

            p.activeLabelObserver = new MutationObserver(() => {
                            if (p.shouldDebugPart5('activeLabelObserver', 250)) {
                            p.debugPart5('activeLabelObserver', {
                                labelState: activeLabel.dataset?.overjoyedState || null,
                                labelText: String(activeLabel.textContent || '').slice(0, 120)
                            });
                            }
                                                        p.syncLblActiveStateFromDom();

              if (!p.canShowHighlights()) {
                p.clearAllHighlightsImmediately();
                                p.updateOverlayVisibility();
                return;
              }

                            p.renderPrompts();
              p.applyHighlight();
              p.updateFromState();
                            p.updateOverlayVisibility();
            });

                        p.activeLabelObserver.observe(activeLabel, {
              childList: true,
              characterData: true,
              subtree: true,
              attributes: true,
                            attributeFilter: ['data-overjoyed-state']
            });
          }

          p.baseOverlay =
            document.querySelector('div[name=""pnlBaseOverlay""]') ||
            document.querySelector('#pnlBaseOverlay') ||
            p.container;

                    p.isRemoteCheckboxChecked = () => {
                        return !!p.state.isRemoteCheckboxChecked;
                    };

          p.updatePointerFromEvent = e => {
                        if (p.isRemoteCheckboxChecked() || !e) {
              return;
            }

            const rect = p.container.getBoundingClientRect();
            const cx = rect.width / 2;
            const cy = rect.height * (350 / 600);
            const radius = rect.height * 0.3;

            let nx = e.clientX - rect.left;
            let ny = e.clientY - rect.top;

            const dx = nx - cx;
            const dy = ny - cy;
            const dist = Math.hypot(dx, dy);

            if (dist > radius) {
              const scale = radius / dist;
              nx = cx + dx * scale;
              ny = cy + dy * scale;
            }

            p.updateSector(nx, ny);
          };

                    p.pointerdownHandler = e => {
                        p.debugPart5('pointerdown', {
                            button: e.button,
                            activeHoverQuadrant: p.state.activeHoverQuadrant ?? null,
                            x: p.x,
                            y: p.y,
                            remote: p.isRemoteCheckboxChecked()
                        });

                                             if (e.button === 0) {
              if (p.state.moveMouseUsingLeftClicksOnly) {
                p.updatePointerFromEvent(e);
              }

              p.state.lcPressQuadrant = p.state.activeHoverQuadrant;
              p.state.lcPressTriggerArmed = true;
              p.state.lcPointerDown = true;
                            p.state.lcPressType = p.getQuadrantInteractionType(p.state.activeHoverQuadrant);
                            p.state.lcRegularHoldAllowed = p.state.lcPressType === 'regular';

                            if (p.state.showLabelsLC && p.state.activeHoverQuadrant != null) {
                                const cycle = p.getOrCreateLcCycle(p.state.activeHoverQuadrant);
                                p.setLcTransitionState(false);
                                p.state.lastEnteredHoverQuadrant = p.state.activeHoverQuadrant;
                                cycle.wasActive = false;
                                cycle.readyForActive = true;

                                if (p.state.lcPressType === 'regular') {
                                    p.cancelLcHeldRelease();
                                    p.state.heldLcQuadrant = p.state.activeHoverQuadrant;
                                    p.state.lcHoldStartedAt = Date.now();
                                }
                            }

              p.syncInteractiveQuadrantState();

              return;
            }

            if (e.button === 2) {
                            p.state.rcPressQuadrant = p.state.activeHoverQuadrant;
                            p.state.rcPressType = p.getQuadrantInteractionType(p.state.activeHoverQuadrant);
              p.state.rcPointerDown = true;
              p.syncInteractiveQuadrantState();
            }
          };

                   p.pointerupHandler = e => {
                        p.debugPart5('pointerup', {
                            button: e.button,
                            activeHoverQuadrant: p.state.activeHoverQuadrant ?? null,
                            x: p.x,
                            y: p.y,
                            remote: p.isRemoteCheckboxChecked()
                        });

            if (e.button === 0) {
              p.state.lcPointerDown = false;
              p.state.lcPressQuadrant = null;
              p.state.lcPressTriggerArmed = false;
                            p.state.lcRegularHoldAllowed = false;
                            p.state.lcPressType = null;
              p.syncInteractiveQuadrantState();
              return;
            }

            if (e.button === 2) {
              p.state.rcPointerDown = false;
                            p.state.rcPressQuadrant = null;
                            p.state.rcPressType = null;
              p.syncInteractiveQuadrantState();
            }
          };

                   p.pointercancelHandler = () => {
                        p.debugPart5('pointercancel', {
                            activeHoverQuadrant: p.state.activeHoverQuadrant ?? null
                        });
            p.state.lcPointerDown = false;
            p.state.rcPointerDown = false;
            p.state.lcPressQuadrant = null;
            p.state.lcPressTriggerArmed = false;
                        p.state.lcRegularHoldAllowed = false;
                        p.state.lcPressType = null;
                        p.state.rcPressQuadrant = null;
                        p.state.rcPressType = null;
            p.state.lcTransitionPending = false;
            p.syncInteractiveQuadrantState();
          };

          p.blurHandler = () => {
                        p.debugPart5('blur', {
                            activeHoverQuadrant: p.state.activeHoverQuadrant ?? null
                        });
            p.state.lcPointerDown = false;
            p.state.rcPointerDown = false;
            p.state.lcPressQuadrant = null;
            p.state.lcPressTriggerArmed = false;
                        p.state.lcRegularHoldAllowed = false;
                        p.state.lcPressType = null;
                        p.state.rcPressQuadrant = null;
                        p.state.rcPressType = null;
            p.state.lcTransitionPending = false;
            p.syncInteractiveQuadrantState();
          };

          p.contextmenuHandler = e => {
            e.preventDefault();
          };

          if (p.baseOverlay) {
            p.baseOverlay.addEventListener('pointerdown', p.pointerdownHandler);
            p.baseOverlay.addEventListener('contextmenu', p.contextmenuHandler);
          }

          window.addEventListener('pointerup', p.pointerupHandler, true);
          window.addEventListener('pointercancel', p.pointercancelHandler, true);
          window.addEventListener('blur', p.blurHandler);

          p.mousemoveHandler = e => {
                        if (p.isRemoteCheckboxChecked() || p.state.moveMouseUsingLeftClicksOnly) {
              return;
            }

            const rect = p.container.getBoundingClientRect();
            const cx = rect.width / 2;
            const cy = rect.height * (350 / 600);
            const radius = rect.height * 0.3;

            let nx = e.clientX - rect.left;
            let ny = e.clientY - rect.top;

            const dx = nx - cx;
            const dy = ny - cy;
            const dist = Math.hypot(dx, dy);

            if (dist > radius) {
              const scale = radius / dist;
              nx = cx + dx * scale;
              ny = cy + dy * scale;
            }

            p.updateSector(nx, ny);
          };

                    p.mousemoveSource = p.baseOverlay || p.container;
                    if (p.mousemoveSource) {
                        p.mousemoveSource.addEventListener('mousemove', p.mousemoveHandler, true);
                    }

                    p.debugPart5('init:bindings', {
                        hasBaseOverlay: !!p.baseOverlay,
                        hasActiveLabel: !!activeLabel,
                        hasMousemoveSource: !!p.mousemoveSource
                    });

          p.applyDefaultFills();
          window.overjoyedPrompts.setState(p.state);
          p.centerPointer();
          p.updateOverlayVisibility();

                    p.debugPart5('init:complete', {
                        x: p.x,
                        y: p.y,
                        activeHoverQuadrant: p.state.activeHoverQuadrant ?? null
                    });

          window.overjoyedOverlay = {
            dispose() {
                            p.debugPart5('dispose', {
                                hadObserver: !!p.activeLabelObserver,
                                hadBaseOverlay: !!p.baseOverlay,
                                hadMousemoveSource: !!p.mousemoveSource
                            });

              if (p.activeLabelObserver) {
                p.activeLabelObserver.disconnect();
                p.activeLabelObserver = null;
              }

                            p.detachLblActiveEventHandlers();
                            p.cancelLblActiveHoldAnimation();

              if (p.baseOverlay && p.pointerdownHandler) {
                p.baseOverlay.removeEventListener('pointerdown', p.pointerdownHandler);
              }

              if (p.baseOverlay && p.contextmenuHandler) {
                p.baseOverlay.removeEventListener('contextmenu', p.contextmenuHandler);
              }

              window.removeEventListener('pointerup', p.pointerupHandler, true);
              window.removeEventListener('pointercancel', p.pointercancelHandler, true);
              window.removeEventListener('blur', p.blurHandler);

                            if (p.mousemoveSource) {
                                p.mousemoveSource.removeEventListener('mousemove', p.mousemoveHandler, true);
                                p.mousemoveSource = null;
                            }
                        p.clearExitHoldProgress();
            p.cancelLcHeldRelease();
            }
          };
        })();");
        }

    }
}