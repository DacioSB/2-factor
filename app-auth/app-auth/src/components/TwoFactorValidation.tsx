import React, { useState, useEffect } from "react";

function Countdown() {
  const [count, setCount] = useState(30);
  const [totp, setTotp] = useState("");
  const [futureTotp, setFutureTotp] = useState("");

  useEffect(() => {
    const start = performance.now();
    
    // Make the initial API call to get the first totp and future totp
    fetch("https://localhost:7076/api/UserConnection/641f2f22f6574a48695288d5")
      .then((response) => response.json())
      .then((data) => {
        const end = performance.now();
        const elapsedTime = end - start;
        
        // Calculate remaining time after discounting the elapsed time
        const remainingSeconds = Math.max(0, data.remainingSeconds - Math.floor(elapsedTime / 1000));
        
        setTotp(data.totp);
        setFutureTotp(data.futureTotp);
        setCount(remainingSeconds);
      })
      .catch((error) => console.log(error));
  }, []);
  

  useEffect(() => {
    if (count === 0) {
      console.log("is zero");
      // After countdown is complete, update totp and reset count to 30
      setTotp(futureTotp);
      setCount(30);

      // Make an async call to the API for the new futureTotp
      fetch("https://localhost:7076/api/UserConnection/641f2f22f6574a48695288d5")
        .then((response) => response.json())
        .then((data) => {
          setFutureTotp(data.futureTotp);
          setCount(data.remainingSeconds);
        })
        .catch((error) => console.log(error));
    }
  }, [count]);

  useEffect(() => {
    // Continuously decrease the count by 1 second
    const intervalId = setInterval(() => {
      setCount((prevCount) => prevCount - 1);
    }, 1000);

    // Clear the interval when count becomes zero
    if (count === 0) {
      clearInterval(intervalId);
    }

    return () => clearInterval(intervalId);
  }, [count]);

  return (
    <div>
      <h1>Countdown: {count}</h1>
      <h2>TOTP: {totp}</h2>
    </div>
  );
}

export default Countdown;
