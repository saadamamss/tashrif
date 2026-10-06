import {
  HubConnectionBuilder,
  HubConnection,
  LogLevel,
  HttpTransportType,
} from "@microsoft/signalr";

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig();

  // Derive hub URL from apiBaseUrl: "http://localhost:5001/api" → "http://localhost:5001"
  const baseUrl = config.public.apiBaseUrl.replace(/\/api\/?$/, "");
  const hubUrl = `${baseUrl}/hubs/notifications`;

  const connection: HubConnection = new HubConnectionBuilder()
    .withUrl(hubUrl, {
      withCredentials: true,
      transport:
        HttpTransportType.WebSockets |
        HttpTransportType.ServerSentEvents |
        HttpTransportType.LongPolling,
    })
    .configureLogging(LogLevel.Information)
    .withAutomaticReconnect({
      nextRetryDelayInMilliseconds: (retryContext) => {
        if (retryContext.elapsedMilliseconds < 120000) {
          return Math.min(
            1000 * Math.pow(2, retryContext.previousRetryCount),
            60000
          );
        }
        return null;
      },
    })
    .build();

  return {
    provide: {
      signalr: connection,
    },
  };
});
