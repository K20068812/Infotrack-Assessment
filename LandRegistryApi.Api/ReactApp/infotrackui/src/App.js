import { useState, useEffect } from "react";

const App = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [targetUrl, setTargetUrl] = useState("");
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [history, setHistory] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(false);

  const API_BASE = "/api";

  const handleSearch = async () => {
    if (!searchQuery || !targetUrl) {
      setError("Please enter both search keywords and target URL.");
      return;
    }

    if (loading) return;

    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const response = await fetch(`${API_BASE}/check-ranking`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          searchQuery,
          targetUrl,
        }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.Error || "Failed to check ranking");
      }

      const data = await response.json();
      setResult(data);
      loadHistory();
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const loadHistory = async () => {
    if (!targetUrl) return;

    setHistoryLoading(true);
    try {
      const encodedUrl = encodeURIComponent(targetUrl);
      const response = await fetch(
        `${API_BASE}/ranking-history/${encodedUrl}?days=30`
      );

      if (response.ok) {
        const data = await response.json();
        setHistory(data);
      }
    } catch (err) {
      console.error("Failed to load history:", err);
    } finally {
      setHistoryLoading(false);
    }
  };

  // useEffect(() => {
  //   loadHistory();
  // }, [targetUrl]);

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString("en-GB", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const getPositionColor = (positions) => {
    if (!positions || positions === "0") return "red";
    const nums = positions.split(",").map(Number);
    const bestPosition = Math.min(...nums);
    if (bestPosition <= 10) return "green";
    if (bestPosition <= 30) return "orange";
    return "red";
  };

  return (
    <div>
      <h1>Ranking Checker</h1>

      <div>
        <h2>Search Form</h2>
        <div>
          <label>Search Keywords:</label>
          <br />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="e.g., land registry searches"
            size="50"
          />
        </div>
        <br />
        <div>
          <label>Target Website URL:</label>
          <br />
          <input
            type="text"
            value={targetUrl}
            onChange={(e) => setTargetUrl(e.target.value)}
            placeholder="e.g., www.infotrack.co.uk"
            size="50"
          />
        </div>
        <br />
        <button onClick={handleSearch} disabled={loading}>
          {loading ? "Checking Rankings..." : "Check Rankings"}
        </button>
      </div>

      {error && (
        <div
          style={{
            color: "red",
            border: "1px solid red",
            padding: "10px",
            margin: "10px 0",
          }}
        >
          <strong>Error:</strong> {error}
        </div>
      )}

      {result && (
        <div
          style={{
            border: "1px solid #ccc",
            padding: "10px",
            margin: "10px 0",
          }}
        >
          <h2>Search Results</h2>
          <p>
            <strong>Search Query:</strong> {result.searchQuery}
          </p>
          <p>
            <strong>Target URL:</strong> {result.targetUrl}
          </p>
          <p>
            <strong>Search Date:</strong> {formatDate(result.searchDate)}
          </p>
          <p>
            <strong>Positions Found:</strong>{" "}
            <span style={{ color: getPositionColor(result.positions) }}>
              {result.positions === "0" ? "Not Found" : result.positions}
            </span>
          </p>

          {result.foundPositions && result.foundPositions.length > 0 && (
            <div>
              <strong>Position Details:</strong>
              {result.foundPositions.map((pos, index) => (
                <span
                  key={index}
                  style={{
                    margin: "0 5px",
                    color: getPositionColor(pos.toString()),
                  }}
                >
                  #{pos}
                </span>
              ))}
            </div>
          )}
        </div>
      )}

      <div
        style={{ border: "1px solid #ccc", padding: "10px", margin: "10px 0" }}
      >
        <h2>
          Ranking History (Last 30 Days)
          <button
            onClick={loadHistory}
            disabled={historyLoading}
            style={{ marginLeft: "10px" }}
          >
            {historyLoading ? "Loading..." : "Refresh"}
          </button>
        </h2>

        {historyLoading ? (
          <p>Loading history...</p>
        ) : history.length > 0 ? (
          <table
            border="1"
            style={{ width: "100%", borderCollapse: "collapse" }}
          >
            <thead>
              <tr>
                <th>Date</th>
                <th>Search Query</th>
                <th>Positions</th>
                <th>Best Position</th>
              </tr>
            </thead>
            <tbody>
              {history.map((item, index) => (
                <tr key={index}>
                  <td>{formatDate(item.searchDate)}</td>
                  <td>{item.searchQuery}</td>
                  <td style={{ color: getPositionColor(item.positions) }}>
                    {item.positions === "0" ? "Not Found" : item.positions}
                  </td>
                  <td>
                    {item.foundPositions && item.foundPositions.length > 0 ? (
                      <span
                        style={{
                          color: getPositionColor(
                            Math.min(...item.foundPositions).toString()
                          ),
                        }}
                      >
                        #{Math.min(...item.foundPositions)}
                      </span>
                    ) : (
                      "-"
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <p>No ranking history found. Perform a search to start tracking!</p>
        )}
      </div>
    </div>
  );
};

export default App;
