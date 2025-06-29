const API_BASE = "/api";

const checkRanking = async (searchQuery, targetUrl) => {
  if (!searchQuery || !targetUrl) {
    throw new Error("Please enter both search keywords and target URL.");
  }

  const response = await fetch(`${API_BASE}/check-ranking`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ searchQuery, targetUrl }),
  });

  if (!response.ok) {
    throw new Error("Failed to check ranking");
  }

  return response.json();
};

const getRankingHistory = async (searchQuery, targetUrl) => {
  if (!searchQuery || !targetUrl) {
    throw new Error("Please enter both search keywords and target URL.");
  }

  const response = await fetch(
    `${API_BASE}/ranking-history?targetUrl=${encodeURIComponent(
      targetUrl
    )}&searchQuery=${encodeURIComponent(searchQuery)}&days=30`
  );

  if (!response.ok) {
    throw new Error("Failed to fetch ranking history");
  }

  return response.json();
};

const getGroupedHistory = async (searchQuery, targetUrl, groupBy) => {
  if (!searchQuery || !targetUrl) {
    throw new Error("Please enter both search keywords and target URL.");
  }

  const response = await fetch(
    `${API_BASE}/grouped-history?targetUrl=${encodeURIComponent(
      targetUrl
    )}&searchQuery=${encodeURIComponent(searchQuery)}&groupBy=${groupBy}`
  );

  if (!response.ok) {
    throw new Error("Failed to fetch grouped history");
  }

  return response.json();
};

export { checkRanking, getRankingHistory, getGroupedHistory };
