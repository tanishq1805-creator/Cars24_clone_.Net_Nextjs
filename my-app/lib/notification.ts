import { getMessaging, getToken, isSupported, onMessage } from "firebase/messaging";
import { app } from "./firebase";

let foregroundListenerRegistered = false;

export async function registerPushNotification() {
  if (typeof window === "undefined") {
    return null;
  }

  const vapidKey = process.env.NEXT_PUBLIC_FIREBASE_VAPID_KEY?.trim();

  // Firebase Web Push VAPID public keys are URL-safe Base64 strings, normally
  // about 87 characters long. Do not attempt a subscription with a placeholder
  // or malformed value.
  if (!vapidKey || !/^[A-Za-z0-9_-]{80,}$/.test(vapidKey)) {
    console.warn("Push notifications are disabled: invalid Firebase VAPID key.");
    return null;
  }

  try {
    const supported = await isSupported();
    if (!supported) {
      return null;
    }

    const permission = await Notification.requestPermission();
    if (permission !== "granted") {
      return null;
    }

    const messaging = getMessaging(app);
    const token = await getToken(messaging, { vapidKey });

    if (!foregroundListenerRegistered) {
      onMessage(messaging, (payload) => {
        const notification = payload.notification;
        if (notification && Notification.permission === "granted") {
          new Notification(notification.title ?? "Cars24", {
            body: notification.body,
            icon: "/logo.png",
          });
        }
      });
      foregroundListenerRegistered = true;
    }

    return token;
  } catch (error) {
    console.warn("Push notification registration failed:", error);
    return null;
  }
}
