# With Ref to https://learn.microsoft.com/en-us/azure/digital-twins/how-to-ingest-iot-hub-data#assign-an-access-role
# These are required in order to authenticate function app to update ADT

#1.
az functionapp identity assign --resource-group <your-resource-group> --name <your-function-app-name>

#2.
az dt role-assignment create --dt-name <your-Azure-Digital-Twins-instance> --assignee "<principal-ID>" --role "Azure Digital Twins Data Owner"

#3.
az eventgrid event-subscription create --name <name-for-hub-event-subscription> --event-delivery-schema eventgridschema --source-resource-id /subscriptions/<your-subscription-ID>/resourceGroups/<your-resource-group>/providers/Microsoft.Devices/IotHubs/<your-IoT-hub> --included-event-types Microsoft.Devices.DeviceTelemetry --endpoint-type azurefunction --endpoint /subscriptions/<your-subscription-ID>/resourceGroups/<your-resource-group>/providers/Microsoft.Web/sites/<your-function-app>/functions/IoTHubtoTwins

az eventgrid event-subscription create 
--name <name-for-hub-event-subscription> 
--event-delivery-schema eventgridschema 
--source-resource-id /subscriptions/<your-subscription-ID>/resourceGroups/<your-resource-group>/providers/Microsoft.Devices/IotHubs/<your-IoT-hub> 
--included-event-types Microsoft.Devices.DeviceTelemetry 
--endpoint-type azurefunction 
--endpoint /subscriptions/<your-subscription-ID>/resourceGroups/<your-resource-group>/providers/Microsoft.Web/sites/<your-function-app>/functions/IoTHubtoTwins