const API_BASE_URL = (
  process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://cars24-clone-net-nextjs-1.onrender.com"
).replace(/\/$/, "");
const BASE_URL = `${API_BASE_URL}/api/Car`;

export type CarDetails = {
  id?: string;
  title: string;
  images: string[];
  price: string;
  recommendedPrice?: number;
  pricingReason?: string;
  emi: string;
  location: string;
  city?: string;
  state?: string;
  latitude?: number;
  longitude?: number;
  specs: {
    year: number;
    km: string;
    fuel: string;
    transmission: string;
    owner: string;
    insurance: string;
  };
  features: string[];
  highlights: string[];
};

export type CarSummary = {
  id: string;
  title: string;
  km: string;
  year: number;
  fuel: string;
  transmission: string;
  owner: string;
  emi: string;
  price: string;
  recommendedPrice?: number;
  pricingReason?: string;
  location: string;
  image: string;
  distance?: number;
};

export type CarSuggestion = {
  id: string;
  title: string;
  year: number;
  fuel: string;
  location: string;
};

export interface CarSearchFilters {
  city?: string;
  query?: string;

  userLatitude?: number;
  userLongitude?: number;

  brands?: string[];
  fuel?: string;
  transmission?: string;

  minPrice?: number;
  maxPrice?: number;

  minMileage?: number;
  maxMileage?: number;

  minYear?: number;
  maxYear?: number;
}

export interface City {
  id: string;
  name: string;
  state: string;
  latitude: number;
  longitude: number;
}



export async function getCities(): Promise<City[]> {
  const response = await fetch(`${API_BASE_URL}/api/city`, {
    cache: "no-store",
  });

  if (!response.ok)
    throw new Error("Unable to load cities");

  return response.json();
}

async function getErrorMessage(response: Response) {
  const body = await response.text();
  return body || `${response.status} ${response.statusText}`;
}

export async function createCar(carDetails: CarDetails, userId: string): Promise<CarDetails> {
  const response = await fetch(`${BASE_URL}?userId=${encodeURIComponent(userId)}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(carDetails),
  });

  if (!response.ok) throw new Error(await getErrorMessage(response));
  return response.json();
}

export async function uploadCarImages(files: File[]): Promise<string[]> {
  const formData = new FormData();
  files.forEach((file) => formData.append("images", file));

  const response = await fetch(`${BASE_URL}/upload`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) throw new Error(await getErrorMessage(response));
  const result: { urls: string[] } = await response.json();
  return result.urls;
}

export async function getCarById(
  id: string,
  city?: string
): Promise<CarDetails> {

  const url =
    city
      ? `${BASE_URL}/${encodeURIComponent(id)}?city=${encodeURIComponent(city)}`
      : `${BASE_URL}/${encodeURIComponent(id)}`;

  const response = await fetch(url);

  if (!response.ok)
    throw new Error(await getErrorMessage(response));

  return response.json();
}

export async function getCarSummaries(): Promise<CarSummary[]> {
  const response = await fetch(`${BASE_URL}/summaries`);
  if (!response.ok) throw new Error(await getErrorMessage(response));
  return response.json();
}

export async function searchCars(filters: CarSearchFilters): Promise<CarSummary[]> {
  const params = new URLSearchParams();

  Object.entries(filters).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      value.forEach((item) => params.append(key, item));
    } else if (value !== undefined && value !== "") {
      params.append(key, String(value));
    }
  });

  const response = await fetch(`${BASE_URL}/search?${params.toString()}`);
  if (!response.ok) throw new Error(await getErrorMessage(response));
  return response.json();
}

export async function getCarSuggestions(query: string): Promise<CarSuggestion[]> {
  if (!query.trim()) return [];

  const response = await fetch(
    `${BASE_URL}/suggestions?query=${encodeURIComponent(query)}`,
  );

  if (!response.ok) return [];
  return response.json();
}
