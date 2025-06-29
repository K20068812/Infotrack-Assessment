import { useEffect, useState, useMemo, memo } from "react";
import {
  LineChart,
  XAxis,
  YAxis,
  Tooltip,
  Line,
  ResponsiveContainer,
  CartesianGrid,
} from "recharts";
import { checkRanking, getGroupedHistory, getRankingHistory } from "./Api";

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
        <Line
          type="monotone"
          dataKey="avgRank"
          stroke="#28a745"
          strokeWidth={2}
          dot={false}
          activeDot={{ r: 4 }}
        />
      </LineChart>
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
  const [groupedHistory, setGroupedHistory] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [viewMode, setViewMode] = useState("table");
  const [groupBy, setGroupBy] = useState("day");

  const API_BASE = "/api";

  useEffect(() => {
    if (targetUrl && groupedHistory.length > 0) {
      loadGroupedHistory();
    }
  }, [groupBy]);

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
      const response = await checkRanking(searchQuery, targetUrl);

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.Error || "Failed to check ranking");
      }

      const data = await response.json();
      setResult(data);
      loadHistory();
      loadGroupedHistory();
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
      const response = await getRankingHistory(searchQuery, targetUrl);
      setHistory(response);
    } catch (err) {
      setError("Failed to load ranking history. Please try again later.");
      setHistory([]);
    } finally {
      setHistoryLoading(false);
    }
  };

  const loadGroupedHistory = async () => {
    if (!targetUrl) return;

    try {
      const response = await getGroupedHistory(searchQuery, targetUrl, groupBy);
      setGroupedHistory(response);
    } catch (err) {
      console.error("Failed to load grouped history:", err);
      setGroupedHistory([]);
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

  const formatPeriodDate = (dateString, groupBy) => {
    const date = new Date(dateString);
    switch (groupBy) {
      case "week":
        return (
          date.toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
          }) + " (Week)"
        );
      case "month":
        return date.toLocaleDateString("en-GB", {
          month: "long",
          year: "numeric",
        });
      default:
        return date.toLocaleDateString("en-GB", {
          day: "2-digit",
          month: "2-digit",
          year: "numeric",
        });
    }
  };

  const getPositionColor = (positions) => {
    if (!positions || positions.length === 0) return "red";
    const bestPosition = Math.min(...positions);
    if (bestPosition <= 10) return "green";
    if (bestPosition <= 30) return "orange";
    return "red";
  };

  const chartData = useMemo(() => {
    if (viewMode === "chart" && groupedHistory.length > 0) {
      return groupedHistory
        .slice()
        .reverse()
        .map((item) => ({
          date: formatPeriodDate(item.periodStart, groupBy),
          bestRank: item.bestPosition,
          avgRank: Math.round(item.averagePosition),
          worstRank: item.worstPosition,
        }));
    }

    return history
      .slice()
      .reverse()
      .map((item) => ({
        date: formatDate(item.searchDate),
        bestRank:
          item.positions && item.positions.length > 0
            ? Math.min(...item.positions)
            : 101,
      }));
  }, [history, groupedHistory, viewMode, groupBy]);

  return (
    <div>
      <h1>SEO Ranking Checker</h1>

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
              {result.positions.length === 0
                ? "Not Found"
                : result.positions.join(", ")}
            </span>
          </div>
        </div>
      )}

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
              onClick={() => setViewMode("grouped")}
              style={{
                backgroundColor: viewMode === "grouped" ? "#007bff" : "#f8f9fa",
                color: viewMode === "grouped" ? "white" : "black",
                marginRight: "5px",
              }}
            >
              Grouped View
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
            <button
              onClick={() => {
                loadHistory();
                loadGroupedHistory();
              }}
              disabled={historyLoading}
            >
              {historyLoading ? "Loading..." : "Refresh"}
            </button>
          </div>
        </div>

        {(viewMode === "grouped" || viewMode === "chart") && (
          <div style={{ marginBottom: "20px" }}>
            <label style={{ marginRight: "10px" }}>Group by:</label>
            <select
              value={groupBy}
              onChange={(e) => setGroupBy(e.target.value)}
              style={{ marginRight: "10px" }}
            >
              <option value="day">Day</option>
              <option value="week">Week</option>
              <option value="month">Month</option>
            </select>
          </div>
        )}

        {historyLoading ? (
          <p>Loading history...</p>
        ) : viewMode === "table" ? (
          history.length > 0 ? (
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
                      {item.positions.length === 0
                        ? "Not Found"
                        : item.positions.join(", ")}
                    </td>
                    <td style={{ padding: "8px" }}>
                      {item.positions && item.positions.length > 0 ? (
                        <span
                          style={{
                            backgroundColor: getPositionColor(item.positions),
                            color: "white",
                            padding: "2px 6px",
                            borderRadius: "3px",
                          }}
                        >
                          #{Math.min(...item.positions)}
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
          )
        ) : viewMode === "grouped" ? (
          groupedHistory.length > 0 ? (
            <table
              border="1"
              style={{ width: "100%", borderCollapse: "collapse" }}
            >
              <thead>
                <tr style={{ backgroundColor: "#f8f9fa" }}>
                  <th style={{ padding: "8px" }}>Period</th>
                  <th style={{ padding: "8px" }}>Search Query</th>
                  <th style={{ padding: "8px" }}>Searches</th>
                  <th style={{ padding: "8px" }}>Best Position</th>
                  <th style={{ padding: "8px" }}>Average Position</th>
                  <th style={{ padding: "8px" }}>Worst Position</th>
                </tr>
              </thead>
              <tbody>
                {groupedHistory.map((item, index) => (
                  <tr key={index}>
                    <td style={{ padding: "8px" }}>
                      {formatPeriodDate(
                        item.periodStart,
                        item.groupingPeriod.toLowerCase()
                      )}
                    </td>
                    <td style={{ padding: "8px" }}>{item.searchQuery}</td>
                    <td style={{ padding: "8px" }}>{item.totalSearches}</td>
                    <td style={{ padding: "8px" }}>
                      <span
                        style={{
                          backgroundColor: getPositionColor([
                            item.bestPosition,
                          ]),
                          color: "white",
                          padding: "2px 6px",
                          borderRadius: "3px",
                        }}
                      >
                        #{item.bestPosition}
                      </span>
                    </td>
                    <td style={{ padding: "8px" }}>#{item.averagePosition}</td>
                    <td style={{ padding: "8px" }}>#{item.worstPosition}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p>
              No grouped data available. Perform a search to start tracking!
            </p>
          )
        ) : (
          viewMode === "chart" && (
            <div>
              <div style={{ marginBottom: "40px" }}>
                <h4>
                  Ranking Trend{" "}
                  {groupedHistory.length > 0 ? `(${groupBy} grouping)` : ""}
                </h4>
                <p style={{ fontSize: "14px", color: "#666" }}>
                  Blue line: Best position, Green line: Average position (lower
                  is better)
                </p>
                <MemoizedLineChart data={chartData} />
              </div>
            </div>
          )
        )}
      </div>
    </div>
  );
};

export default App;
