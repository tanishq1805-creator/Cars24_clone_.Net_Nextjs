const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://cars24-clone-net-nextjs-1.onrender.com"
).replace(/\/$/, "");
const BASE_URL = `${API_BASE_URL}/api/Reward`;

export interface Reward {
    id: string;
    title: string;
    description: string;
    pointsRequired: number;
    category: string;
    imageUrl: string;
}

export async function getRewards(): Promise<Reward[]> {
    const response = await fetch(BASE_URL);

    if (!response.ok)
        throw new Error("Failed to load rewards.");

    return response.json();
}

export async function redeemReward(
    userId: string,
    rewardId: string
) {
    const response = await fetch(`${BASE_URL}/redeem`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            userId,
            rewardId
        })
    });

    if (!response.ok) {
        const body = await response.json().catch(() => null);
        throw new Error(body?.message || "Failed to redeem reward.");
    }

    return response.json();
}
