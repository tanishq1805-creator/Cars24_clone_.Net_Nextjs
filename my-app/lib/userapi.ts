const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5278"
).replace(/\/$/, "");

const USER_AUTH_URL = `${API_BASE_URL}/api/UserAuth`;
const NOTIFICATION_URL = `${API_BASE_URL}/api/Notification`;

export type Wallet = {
  balance: number;
  createdAt: string;
  updatedAt: string;
};

export type WalletTransaction = {
  id: string;
  points: number;
  type: string;
  description: string;
  createdAt: string;
};

export type Referral = {
  referralCode: string;
  successfulReferrals: number;
  walletPoints: number;
};

const parseErrorMessage = async (response: Response, fallbackMessage: string) => {
  try {
    const errorData = await response.json();
    return errorData?.message || fallbackMessage;
  } catch {
    return fallbackMessage;
  }
};

export const signup = async (
  email: string,
  password: string,
  userData: { fullName: string; phone: string; referralCode?: string }
) => {
  const response = await fetch(`${USER_AUTH_URL}/signup`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password, ...userData }),
  });

  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to sign up"));
  }

  return response.json();
};

export const login = async (email: string, password: string) => {
  const response = await fetch(`${USER_AUTH_URL}/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });

  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to log in"));
  }

  return response.json();
};

export const getUserById = async (userId: string) => {
  const response = await fetch(`${USER_AUTH_URL}/${userId}`);
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to fetch user"));
  }

  return response.json();
};

export const registerNotificationToken = async (userId: string, token: string) => {
  const response = await fetch(`${NOTIFICATION_URL}/register-token`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ userId, token }),
  });

  if (!response.ok) {
    throw new Error(
      await parseErrorMessage(response, "Failed to register notification token")
    );
  }

  return response.json();
};

export const getWallet = async (userId: string): Promise<Wallet> => {
  const response = await fetch(`${USER_AUTH_URL}/wallet/${userId}`);
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to fetch wallet"));
  }

  return response.json();
};

export const getWalletTransactions = async (
  userId: string
): Promise<WalletTransaction[]> => {
  const response = await fetch(`${USER_AUTH_URL}/wallet/${userId}/transactions`);
  if (!response.ok) {
    throw new Error(
      await parseErrorMessage(response, "Failed to fetch wallet transactions")
    );
  }

  return response.json();
};

export const getReferral = async (userId: string): Promise<Referral> => {
  const response = await fetch(`${USER_AUTH_URL}/referral/${userId}`);
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to fetch referral details"));
  }

  return response.json();
};
