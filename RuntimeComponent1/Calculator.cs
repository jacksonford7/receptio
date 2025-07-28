using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace RuntimeComponent1
{
    public sealed class Calculator : IBackgroundTask
    {
        private BackgroundTaskDeferral backgroundTaskDeferral;
        private AppServiceConnection appServiceConnection;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            this.backgroundTaskDeferral = taskInstance.GetDeferral();

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            appServiceConnection = details.AppServiceConnection;

            appServiceConnection.RequestReceived += OnRequestReceived;
            taskInstance.Canceled += OnTaskCanceled;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDeferral = args.GetDeferral();
            ValueSet message = args.Request.Message;
            ValueSet returnData = new ValueSet();

            string command = message["Command"] as string;      //Add, Subtract, Multiply, Divide
            int? firstNumber = message["Input1"] as int?;
            int? secondNumber = message["Input2"] as int?;
            int? result = 0;

            if (firstNumber.HasValue && secondNumber.HasValue)
            {
                switch (command)
                {
                    case "Add":
                        {
                            result = firstNumber + secondNumber;
                            returnData.Add("Result", result.ToString());
                            returnData.Add("Status", "Complete");
                            break;
                        }
                    case "Subtract":
                        {
                            result = firstNumber - secondNumber;
                            returnData.Add("Result", result.ToString());
                            returnData.Add("Status", "Complete");
                            break;
                        }
                    case "Multiply":
                        {
                            result = firstNumber * secondNumber;
                            returnData.Add("Result", result.ToString());
                            returnData.Add("Status", "Complete");
                            break;
                        }
                    case "Divide":
                        {
                            result = firstNumber / secondNumber;
                            returnData.Add("Result", result.ToString());
                            returnData.Add("Status", "Complete");
                            break;
                        }
                    default:
                        {
                            returnData.Add("Status", "Fail: unknown command");
                            break;
                        }
                }
            }

            await args.Request.SendResponseAsync(returnData);
            messageDeferral.Complete();
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this.backgroundTaskDeferral != null)
            {
                this.backgroundTaskDeferral.Complete();
            }
        }
    }
}
