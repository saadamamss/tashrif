import type { HubConnection, HubConnectionState } from "@microsoft/signalr";

type EventCallback = (...args: any[]) => void;

// No-op stub for SSR — all methods do nothing
const noop = () => {};
const noopAsync = async () => {};

const ssrStub = {
  connection: null,
  isConnected: { value: false },
  connectionState: { value: "Disconnected" },
  start: noopAsync,
  stop: noopAsync,
  on: noop,
  off: noop,
  invoke: noopAsync,
};

export function useSignalr() {
  if (import.meta.server) {
    return ssrStub;
  }

  const nuxtApp = useNuxtApp();
  const connection = nuxtApp.$signalr as HubConnection;

  if (!connection) {
    return ssrStub;
  }

  const isConnected = useState<boolean>("signalr:connected", () => false);
  const connectionState = useState<HubConnectionState>(
    "signalr:state",
    () => "Disconnected" as HubConnectionState
  );

  const registeredEvents = new Map<string, EventCallback>();

  connection.onreconnecting(() => {
    isConnected.value = false;
    connectionState.value = "Reconnecting";
  });

  connection.onreconnected(() => {
    isConnected.value = true;
    connectionState.value = "Connected";
  });

  connection.onclose(() => {
    isConnected.value = false;
    connectionState.value = "Disconnected";
  });

  async function start(): Promise<void> {
    try {
      await connection.start();
      isConnected.value = true;
      connectionState.value = "Connected";
    } catch (err) {
      console.error("[SignalR] Connect failed, retrying in 5s...", err);
      setTimeout(() => start(), 5000);
    }
  }

  async function stop(): Promise<void> {
    await connection.stop();
    isConnected.value = false;
    connectionState.value = "Disconnected";
  }

  function on(eventName: string, callback: EventCallback): void {
    if (registeredEvents.has(eventName)) {
      connection.off(eventName, registeredEvents.get(eventName));
    }
    connection.on(eventName, callback);
    registeredEvents.set(eventName, callback);
  }

  function off(eventName: string): void {
    if (registeredEvents.has(eventName)) {
      connection.off(eventName, registeredEvents.get(eventName));
      registeredEvents.delete(eventName);
    }
  }

  async function invoke<T = void>(
    methodName: string,
    ...args: any[]
  ): Promise<T> {
    return connection.invoke(methodName, ...args) as Promise<T>;
  }

  return {
    connection,
    isConnected,
    connectionState,
    start,
    stop,
    on,
    off,
    invoke,
  };
}
