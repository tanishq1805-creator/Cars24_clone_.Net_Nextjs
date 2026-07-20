const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://cars24-clone-net-nextjs-1.onrender.com"
).replace(/\/$/, "");
const BASE_URL = `${API_BASE_URL}/api/Booking`;

type BookingRequest = Record<string, unknown>;

const getErrorMessage = async (response: Response, fallback: string) => {
  try {
    const body = await response.json();
    return body?.message ?? body?.title ?? fallback;
  } catch {
    return fallback;
  }
};

export const createBooking = async (userId: string, booking: BookingRequest) => {
  const response = await fetch(`${BASE_URL}?userId=${encodeURIComponent(userId)}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(booking),
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Failed to create booking"));
  }

  return response.json();
};

export const getBookingbyid = async (id: string) => {
  const response = await fetch(`${BASE_URL}/${encodeURIComponent(id)}`);
  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Booking not found"));
  }

  return response.json();
};

export const getBookingbyuser = async (userId: string) => {
  const response = await fetch(
    `${BASE_URL}/user/${encodeURIComponent(userId)}/bookings`
  );
  if (!response.ok) {
    throw new Error(await getErrorMessage(response, "Failed to fetch bookings"));
  }

  return response.json();
};
