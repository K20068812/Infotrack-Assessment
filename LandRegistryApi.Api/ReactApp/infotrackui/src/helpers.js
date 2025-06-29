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
  if (!positions || positions.length === 0) return "red";
  const bestPosition = Math.min(...positions);
  if (bestPosition <= 10) return "green";
  if (bestPosition <= 30) return "orange";
  return "red";
};

export { formatPeriodDate, formatDate, getPositionColor };
