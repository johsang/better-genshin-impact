﻿using BetterGenshinImpact.Core.Simulator;
using BetterGenshinImpact.GameTask.AutoGeniusInvokation.Exception;
using BetterGenshinImpact.GameTask.AutoSkip.Assets;
using BetterGenshinImpact.GameTask.AutoWood.Utils;
using BetterGenshinImpact.View.Drawable;
using Microsoft.Extensions.Logging;
using System;
using static BetterGenshinImpact.GameTask.Common.TaskControl;
using static Vanara.PInvoke.User32;

namespace BetterGenshinImpact.GameTask.AutoSkip;

public class OneKeyExpeditionTask
{
    public void Run(AutoSkipAssets assets)
    {
        try
        {
            // 1.全部领取
            var content = CaptureToContent();
            content.CaptureRectArea.Find(assets.CollectRo, ra =>
            {
                ra.ClickCenter();
                Logger.LogInformation("探索派遣：{Text}", "全部领取");
                Sleep(1100);
                // 2.重新派遣
                NewRetry.Do(() =>
                {
                    Sleep(1);
                    content = CaptureToContent();
                    var ra2 = content.CaptureRectArea.Find(assets.ReRo);
                    if (ra2.IsEmpty())
                    {
                        throw new RetryException("未检测到弹出菜单");
                    }
                    else
                    {
                        ra2.ClickCenter();
                        Logger.LogInformation("探索派遣：{Text}", "再次派遣");
                    }
                }, TimeSpan.FromSeconds(1), 3);

                // 3.退出派遣页面 ESC
                Sleep(500);
                Simulation.SendInputEx.Keyboard.KeyPress(VK.VK_ESCAPE);
                Logger.LogInformation("探索派遣：{Text}", "完成");
            });
        }
        catch (Exception e)
        {
            Logger.LogInformation(e.Message);
        }
        finally
        {
            VisionContext.Instance().DrawContent.ClearAll();
        }
    }
}