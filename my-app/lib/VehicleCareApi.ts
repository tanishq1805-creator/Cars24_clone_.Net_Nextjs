const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://cars24-clone-net-nextjs-1.onrender.com"
).replace(/\/$/, "");
const BASE_URL = `${API_BASE_URL}/api`;

export interface VehicleCareResponse {
    success: boolean;
    data: {
        condition: string;
        monthlyCost: number;
        annualCost: number;
        nextServiceInKm: number;
        predictions: string[];
    };
}

export async function getVehicleCare(id: string) {
    const response = await fetch(`${BASE_URL}/vehicle-care/${id}`);

    if (!response.ok)
        throw new Error("Failed to load vehicle care.");

    return response.json() as Promise<VehicleCareResponse>;
}
