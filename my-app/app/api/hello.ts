const BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL
  ? `${process.env.NEXT_PUBLIC_API_BASE_URL}/api/Car`
  : "http://localhost:5278/api/Car";

const parseErrorMessage = async (
  response: Response,
  fallbackMessage: string
) => {
  try {
    const errorData = await response.json();
    return errorData?.message || fallbackMessage;
  } catch {
    try {
      const text = await response.text();
      return text || fallbackMessage;
    } catch {
      return fallbackMessage;
    }
  }
};

export const getCarSummaries = async () => {
  const response = await fetch(`${BASE_URL}/summaries`);

  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to fetch cars"));
  }

  return response.json();
};

export const searchCars = async (params: {
  query?: string;
  fuel?: string;
  transmission?: string;
  minYear?: number;
  maxYear?: number;
  minMileage?: number;
  maxMileage?: number;
}) => {
  const search = new URLSearchParams();

  if (params.query) search.append("query", params.query);
  if (params.fuel) search.append("fuel", params.fuel);
  if (params.transmission) search.append("transmission", params.transmission);
  if (params.minYear)
    search.append("minYear", params.minYear.toString());
  if (params.maxYear)
    search.append("maxYear", params.maxYear.toString());
  if (params.minMileage)
    search.append("minMileage", params.minMileage.toString());
  if (params.maxMileage)
    search.append("maxMileage", params.maxMileage.toString());

  const response = await fetch(`${BASE_URL}/search?${search.toString()}`);

  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Search failed"));
  }

  return response.json();
};

export const getSuggestions = async (
  query: string,
  limit: number = 6
) => {
  const response = await fetch(
    `${BASE_URL}/suggestions?query=${encodeURIComponent(
      query
    )}&limit=${limit}`
  );

  if (!response.ok) {
    throw new Error(
      await parseErrorMessage(response, "Failed to fetch suggestions")
    );
  }

  return response.json();
};

export const getCarById = async (id: string) => {
  const response = await fetch(`${BASE_URL}/${id}`);

  if (!response.ok) {
    throw new Error(await parseErrorMessage(response, "Failed to fetch car"));
  }

  return response.json();
};