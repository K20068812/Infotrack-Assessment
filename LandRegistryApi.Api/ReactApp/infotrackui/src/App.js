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
import { formatDate, formatPeriodDate, getPositionColor } from "./helpers";

const MemoizedLineChart = memo(({ data }) => (
  <div className="chart-container">
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
          name="Best Position"
        />
        <Line
          type="monotone"
          dataKey="avgRank"
          stroke="#28a745"
          strokeWidth={2}
          dot={false}
          activeDot={{ r: 4 }}
          name="Average Position"
        />
        <Line
          type="monotone"
          dataKey="worstRank"
          stroke="#dc3545"
          strokeWidth={2}
          dot={false}
          activeDot={{ r: 4 }}
          name="Worst Position"
        />
      </LineChart>
    </ResponsiveContainer>
  </div>
));

const App = () => {
  const [searchQuery, setSearchQuery] = useState("");
  const [targetUrl, setTargetUrl] = useState("");
  const [searchEngine, setSearchEngine] = useState("Google");
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState(null);
  const [error, setError] = useState(null);
  const [history, setHistory] = useState([]);
  const [groupedHistory, setGroupedHistory] = useState([]);
  const [historyLoading, setHistoryLoading] = useState(false);
  const [viewMode, setViewMode] = useState("table");
  const [groupBy, setGroupBy] = useState("day");

  useEffect(() => {
    if (targetUrl && groupedHistory.length > 0) {
      loadGroupedHistory();
    }
  }, [groupBy, searchEngine]);

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
      const response = await checkRanking(searchQuery, targetUrl, searchEngine);

      setResult(response);
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
      const response = await getRankingHistory(
        searchQuery,
        targetUrl,
        searchEngine
      );
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
      const response = await getGroupedHistory(
        searchQuery,
        targetUrl,
        searchEngine,
        groupBy
      );
      setGroupedHistory(response);
    } catch (err) {
      console.error("Failed to load grouped history:", err);
      setGroupedHistory([]);
    }
  };

  const chartData = useMemo(() => {
    if (viewMode === "chart" && groupedHistory.length > 0) {
      return groupedHistory.slice().map((item) => ({
        date: formatPeriodDate(item.periodStart, groupBy),
        bestRank: item.bestPosition,
        avgRank: Math.round(item.averagePosition),
        worstRank: item.worstPosition,
      }));
    }

    return history.slice().map((item) => ({
      date: formatDate(item.searchDate),
      bestRank:
        item.positions && item.positions.length > 0
          ? Math.min(...item.positions)
          : 101,
    }));
  }, [history, groupedHistory, viewMode, groupBy]);

  return (
    <div className="container">
      <h1>Ranking Checker</h1>

      <div className="section-box">
        <h2>Search Form</h2>

        <div className="form-group">
          <label>Search Engine:</label>
          <div className="radio-group">
            <label className="radio-option">
              <input
                type="radio"
                value="Google"
                checked={searchEngine === "Google"}
                onChange={(e) => setSearchEngine(e.target.value)}
                className="radio-input"
              />
              Google
            </label>
            <label className="radio-option">
              <input
                type="radio"
                value="Bing"
                checked={searchEngine === "Bing"}
                onChange={(e) => setSearchEngine(e.target.value)}
                className="radio-input"
              />
              Bing
            </label>
          </div>
        </div>

        <div className="form-group">
          <label>Search Keywords:</label>
          <br />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="e.g., land registry searches"
            className="text-input"
          />
        </div>

        <div className="form-group">
          <label>Target Website URL:</label>
          <br />
          <input
            type="text"
            value={targetUrl}
            onChange={(e) => setTargetUrl(e.target.value)}
            placeholder="e.g., www.infotrack.co.uk"
            className="text-input"
          />
        </div>

        <button
          onClick={handleSearch}
          disabled={loading}
          className="primary-button"
        >
          {loading ? "Checking Rankings..." : "Check Rankings"}
        </button>
      </div>

      {error && <div className="error-box">Error: {error}</div>}

      {result && (
        <div className="result-box">
          <h3>Search Results</h3>
          <div className="result-item">
            <strong>Search Engine:</strong>{" "}
            {result.searchEngine || searchEngine}
          </div>
          <div className="result-item">
            <strong>Search Query:</strong> {result.searchQuery}
          </div>
          <div className="result-item">
            <strong>Target URL:</strong> {result.targetUrl}
          </div>
          <div className="result-item">
            <strong>Search Date:</strong> {formatDate(result.searchDate)}
          </div>
          <div className="result-item">
            <strong>Positions Found:</strong>{" "}
            <span style={{ color: getPositionColor(result.positions) }}>
              {result.positions.length === 0
                ? "Not Found"
                : result.positions.join(", ")}
            </span>
          </div>
        </div>
      )}

      <div className="section-box">
        <div className="header-controls">
          <h3>
            Ranking History (Last 30 Days) -{" "}
            {searchEngine.charAt(0).toUpperCase() + searchEngine.slice(1)}
          </h3>

          <div className="view-controls">
            <button
              onClick={() => setViewMode("table")}
              className={`view-button ${viewMode === "table" ? "active" : ""}`}
            >
              Table View
            </button>
            <button
              onClick={() => setViewMode("grouped")}
              className={`view-button ${
                viewMode === "grouped" ? "active" : ""
              }`}
            >
              Grouped View
            </button>
            <button
              onClick={() => setViewMode("chart")}
              className={`view-button ${viewMode === "chart" ? "active" : ""}`}
            >
              Chart View
            </button>
            <button
              onClick={() => {
                loadHistory();
                loadGroupedHistory();
              }}
              disabled={historyLoading}
              className="refresh-button"
            >
              {historyLoading ? "Loading..." : "Refresh"}
            </button>
          </div>
        </div>

        {(viewMode === "grouped" || viewMode === "chart") && (
          <div className="group-controls">
            <label className="group-label">Group by:</label>
            <select
              value={groupBy}
              onChange={(e) => setGroupBy(e.target.value)}
              className="group-select"
            >
              <option value="day">Day</option>
              <option value="week">Week</option>
            </select>
          </div>
        )}

        {historyLoading ? (
          <p className="loading-text">Loading history...</p>
        ) : viewMode === "table" ? (
          history.length > 0 ? (
            <table className="data-table">
              <thead>
                <tr className="table-header">
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
                      {item.positions.length === 0
                        ? "Not Found"
                        : item.positions.join(", ")}
                    </td>
                    <td>
                      {item.positions && item.positions.length > 0 ? (
                        <span
                          className="position-badge"
                          style={{
                            backgroundColor: getPositionColor(item.positions),
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
            <p className="empty-state">No ranking history found</p>
          )
        ) : viewMode === "grouped" ? (
          groupedHistory.length > 0 ? (
            <table className="data-table">
              <thead>
                <tr className="table-header">
                  <th>Period</th>
                  <th>Search Query</th>
                  <th>Searches</th>
                  <th>Best Position</th>
                  <th>Average Position</th>
                  <th>Worst Position</th>
                </tr>
              </thead>
              <tbody>
                {groupedHistory.map((item, index) => (
                  <tr key={index}>
                    <td>
                      {formatPeriodDate(
                        item.periodStart,
                        item.groupingPeriod.toLowerCase()
                      )}
                    </td>
                    <td>{item.searchQuery}</td>
                    <td>{item.totalSearches}</td>
                    <td>
                      <span
                        className="position-badge"
                        style={{
                          backgroundColor: getPositionColor([
                            item.bestPosition,
                          ]),
                        }}
                      >
                        #{item.bestPosition}
                      </span>
                    </td>
                    <td>#{item.averagePosition}</td>
                    <td>#{item.worstPosition}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <p className="empty-state">
              No grouped data available. Perform a search to start tracking
            </p>
          )
        ) : (
          viewMode === "chart" && (
            <div>
              <div>
                <h4 className="chart-title">
                  Ranking Trend{" "}
                  {groupedHistory.length > 0 ? `(${groupBy} grouping)` : ""}
                </h4>
                <p className="chart-subtitle">Best position chart</p>
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
