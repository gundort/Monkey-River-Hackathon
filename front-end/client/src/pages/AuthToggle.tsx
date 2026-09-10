import React, { useState, useRef, type FormEvent } from "react";
import "../style/AuthToggle.css";

interface FormState {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirm: string;
}

interface Errors {
  firstName?: string;
  lastName?: string;
  email?: string;
  password?: string;
  confirm?: string;
}

function AuthToggle() {
  const [isRegister, setIsRegister] = useState<boolean>(false);
  const [form, setForm] = useState<FormState>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirm: "",
  });
  const [errors, setErrors] = useState<Errors>({});
  const [showPassword, setShowPassword] = useState<boolean>(false);
  const [showConfirm, setShowConfirm] = useState<boolean>(false);
  const eyeRefs = [useRef<HTMLDivElement>(null), useRef<HTMLDivElement>(null)];

  const handleToggle = () => {
    setIsRegister((prev) => !prev);
    setForm({
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirm: "",
    });
    setErrors({});
  };

  const validate = (): boolean => {
    const newErrors: Errors = {};
    if (isRegister) {
      if (!form.firstName) newErrors.firstName = "Required";
      if (!form.lastName) newErrors.lastName = "Required";
      if (form.password.length < 8 || !/[!@#$%^&*]/.test(form.password))
        newErrors.password = "8+ chars & special";
      if (form.password !== form.confirm) newErrors.confirm = "Must match";
    }
    if (!/.+@.+\..+/.test(form.email)) newErrors.email = "Invalid email";
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!validate()) return;

    if (isRegister) {
    console.log( "Registering", form);
       // Registration request
        const response = await fetch('http://localhost:5000/api/auth/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            firstName: form.firstName,
            lastName: form.lastName,
            email: form.email,
            password: form.password,
          }),
        });

        const data = await response.json();

        if (response.ok) {
          console.log("Registration successful", data);
          // Handle successful registration (e.g., show success message, redirect)
          alert("Registration successful! Please check your email to verify your account.");
        } else {
          // Handle server validation errors
          setErrors({
            general: data.message || "Registration failed"
          });
        }

    } else {
    console.log("Logging in", form);

    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const focusEyes = (action: "open" | "close" | "peek") => {
    eyeRefs.forEach((ref) => {
      if (ref.current) {
        if (action === "open") {
          ref.current.classList.remove("closed", "peeking");
        } else if (action === "close") {
          ref.current.classList.add("closed");
          ref.current.classList.remove("peeking");
        } else if (action === "peek") {
          ref.current.classList.add("peeking");
          ref.current.classList.remove("closed");
        }
      }
    });
  };

  return (
    <div className="auth-container">
      <div className="balls">
        <div className="ball eye-ball" ref={eyeRefs[0]} />
        <div className="ball eye-ball" ref={eyeRefs[1]} />
      </div>

      <div className="toggle-buttons">
        <button className={!isRegister ? "active" : ""} onClick={handleToggle}>
          Login
        </button>
        <button className={isRegister ? "active" : ""} onClick={handleToggle}>
          Register
        </button>
      </div>

      <form onSubmit={handleSubmit}>
        {isRegister && (
          <>
            <input
              name="firstName"
              placeholder="First Name"
              value={form.firstName}
              onChange={handleChange}
              onFocus={() => focusEyes("open")}
            />
            {errors.firstName && <small>{errors.firstName}</small>}
            <input
              name="lastName"
              placeholder="Last Name"
              value={form.lastName}
              onChange={handleChange}
              onFocus={() => focusEyes("open")}
            />
            {errors.lastName && <small>{errors.lastName}</small>}
          </>
        )}

        <input
          name="email"
          placeholder="Email Address"
          value={form.email}
          onChange={handleChange}
          onFocus={() => focusEyes("open")}
        />
        {errors.email && <small>{errors.email}</small>}

        <div className="password-wrapper">
          <input
            type={showPassword ? "text" : "password"}
            name="password"
            placeholder="Password"
            value={form.password}
            onChange={handleChange}
            onFocus={() => focusEyes("close")}
          />
          <span
            className="password-toggle"
            onClick={() => {
              setShowPassword((prev) => !prev);
              focusEyes("peek");
            }}
          >
            {showPassword ? "🙈" : "👁️"}
          </span>
        </div>
        {errors.password && <small>{errors.password}</small>}

        {isRegister && (
          <>
            <div className="password-wrapper">
              <input
                type={showConfirm ? "text" : "password"}
                name="confirm"
                placeholder="Confirm Password"
                value={form.confirm}
                onChange={handleChange}
                onFocus={() => focusEyes("close")}
              />
              <span
                className="password-toggle"
                onClick={() => {
                  setShowConfirm((prev) => !prev);
                  focusEyes("peek");
                }}
              >
                {showConfirm ? "🙈" : "👁️"}
              </span>
            </div>
            {errors.confirm && <small>{errors.confirm}</small>}
          </>
        )}

        {!isRegister && <a href="/forgot">Forgot password?</a>}

        <button type="submit">{isRegister ? "Register" : "Log in"}</button>
      </form>
    </div>
  );
}

export default AuthToggle;