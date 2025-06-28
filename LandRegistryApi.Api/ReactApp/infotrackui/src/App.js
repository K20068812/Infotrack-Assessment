import { useState, useEffect, useMemo, memo } from "react";
import {
  LineChart,
  XAxis,
  YAxis,
  Tooltip,
  Line,
  ResponsiveContainer,
  ScatterChart,
  Scatter,
  CartesianGrid,
} from "recharts";

const MemoizedLineChart = memo(({ data }) => (
  <div style={{ width: "100%", height: "300px" }}>
    <ResponsiveContainer>
      <LineChart data={data}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="date" />
        <YAxis domain={[0, "dataMax + 10"]} reversed={true} />
        <Tooltip />
        <Line
          type="monotone"
          dataKey="bestRank"
          stroke="#007bff"
          strokeWidth={2}
          dot={false}
          activeDot={{ r: 4 }}
        />
      </LineChart>
    </ResponsiveContainer>
  </div>
));

const MemoizedScatterChart = memo(({ data }) => (
  <div style={{ width: "100%", height: "300px" }}>
    <ResponsiveContainer>
      <ScatterChart data={data}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="date" />
        <YAxis dataKey="position" domain={[0, "dataMax + 5"]} reversed={true} />
        <Tooltip />
        <Scatter dataKey="position" fill="#28a745" />
      </ScatterChart>
    </ResponsiveContainer>
  </div>
));

const App = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [targetUrl, setTargetUrl] = useState("");
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [history, setHistory] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [viewMode, setViewMode] = useState("table"); // "table" or "chart"

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
      setError(err.message || "An unexpected error occurred.");
      setResult(null);
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
      setError("Failed to load ranking history. Please try again later.");
      setHistory([]);
    } finally {
      setHistoryLoading(false);
    }
  };

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

  // Only memoize the chart data that gets passed to Recharts
  const chartData = useMemo(() => {
    return history
      .slice()
      .reverse()
      .map((item) => ({
        date: formatDate(item.searchDate),
        bestRank:
          item.foundPositions && item.foundPositions.length > 0
            ? Math.min(...item.foundPositions)
            : 101,
      }));
  }, [history]);

  const scatterData = useMemo(() => {
    return history
      .slice()
      .reverse()
      .flatMap((item, dateIndex) =>
        item.foundPositions
          ? item.foundPositions.map((position) => ({
              date: formatDate(item.searchDate),
              position: position,
              dateIndex: dateIndex,
            }))
          : []
      );
  }, [history]);

  return (
    <div>
      <h1>SEO Ranking Checker</h1>

      {/* Search Form */}
      <div
        style={{ border: "1px solid #ccc", padding: "20px", margin: "20px 0" }}
      >
        <h2>Search Form</h2>

        <div style={{ marginBottom: "15px" }}>
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

        <div style={{ marginBottom: "15px" }}>
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

        <button onClick={handleSearch} disabled={loading}>
          {loading ? "Checking Rankings..." : "Check Rankings"}
        </button>
      </div>

      {/* Error Display */}
      {error && (
        <div
          style={{
            backgroundColor: "#ffebee",
            padding: "10px",
            border: "1px solid red",
            margin: "10px 0",
          }}
        >
          Error: {error}
        </div>
      )}

      {/* Results Display */}
      {result && (
        <div
          style={{
            border: "1px solid #ccc",
            padding: "15px",
            margin: "20px 0",
          }}
        >
          <h3>Search Results</h3>
          <div>
            <strong>Search Query:</strong> {result.searchQuery}
          </div>
          <div>
            <strong>Target URL:</strong> {result.targetUrl}
          </div>
          <div>
            <strong>Search Date:</strong> {formatDate(result.searchDate)}
          </div>
          <div>
            <strong>Positions Found:</strong>{" "}
            <span style={{ color: getPositionColor(result.positions) }}>
              {result.positions === "0" ? "Not Found" : result.positions}
            </span>
          </div>
        </div>
      )}

      {/* History Section */}
      <div
        style={{ border: "1px solid #ccc", padding: "20px", margin: "20px 0" }}
      >
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            marginBottom: "20px",
          }}
        >
          <h3>Ranking History (Last 30 Days)</h3>

          <div>
            <button
              onClick={() => setViewMode("table")}
              style={{
                backgroundColor: viewMode === "table" ? "#007bff" : "#f8f9fa",
                color: viewMode === "table" ? "white" : "black",
                marginRight: "5px",
              }}
            >
              Table View
            </button>
            <button
              onClick={() => setViewMode("chart")}
              style={{
                backgroundColor: viewMode === "chart" ? "#007bff" : "#f8f9fa",
                color: viewMode === "chart" ? "white" : "black",
                marginRight: "10px",
              }}
            >
              Chart View
            </button>
            <button onClick={loadHistory} disabled={historyLoading}>
              {historyLoading ? "Loading..." : "Refresh"}
            </button>
          </div>
        </div>

        {historyLoading ? (
          <p>Loading history...</p>
        ) : history.length > 0 ? (
          viewMode === "table" ? (
            <table
              border="1"
              style={{ width: "100%", borderCollapse: "collapse" }}
            >
              <thead>
                <tr style={{ backgroundColor: "#f8f9fa" }}>
                  <th style={{ padding: "8px" }}>Date</th>
                  <th style={{ padding: "8px" }}>Search Query</th>
                  <th style={{ padding: "8px" }}>Positions</th>
                  <th style={{ padding: "8px" }}>Best Position</th>
                </tr>
              </thead>
              <tbody>
                {history.map((item, index) => (
                  <tr key={index}>
                    <td style={{ padding: "8px" }}>
                      {formatDate(item.searchDate)}
                    </td>
                    <td style={{ padding: "8px" }}>{item.searchQuery}</td>
                    <td
                      style={{
                        padding: "8px",
                        color: getPositionColor(item.positions),
                      }}
                    >
                      {item.positions === "0" ? "Not Found" : item.positions}
                    </td>
                    <td style={{ padding: "8px" }}>
                      {item.foundPositions && item.foundPositions.length > 0 ? (
                        <span
                          style={{
                            backgroundColor: getPositionColor(item.positions),
                            color: "white",
                            padding: "2px 6px",
                            borderRadius: "3px",
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
            viewMode === "chart" && (
              <div>
                {/* Best Rankings Line Chart */}
                <div style={{ marginBottom: "40px" }}>
                  <h4>Best Ranking Trend</h4>
                  <MemoizedLineChart data={chartData} />
                </div>

                {/* All Positions Scatter Chart */}
                <div>
                  <h4>All Ranking Positions</h4>
                  <MemoizedScatterChart data={scatterData} />
                  <p
                    style={{
                      fontSize: "12px",
                      color: "#666",
                      fontStyle: "italic",
                    }}
                  >
                    Each dot shows a ranking position found on that date
                  </p>
                </div>
              </div>
            )
          )
        ) : (
          <p>No ranking history found. Perform a search to start tracking!</p>
        )}
      </div>
    </div>
  );
};

export default App;
