const API_BASE = "http://localhost:5126/api"; 

let authToken = sessionStorage.getItem("token") || null;
let currentUsername = sessionStorage.getItem("username") || null;
let currentCourseId = null;

const loginView = document.getElementById("loginView");
const dashboardView = document.getElementById("dashboardView");
const courseDetailView = document.getElementById("courseDetailView");
const userBar = document.getElementById("userBar");
const usernameLabel = document.getElementById("usernameLabel");
const loginForm = document.getElementById("loginForm");
const loginError = document.getElementById("loginError");
const toast = document.getElementById("toast");

function showToast(message, isError = false) {
  toast.textContent = message;
  toast.className = `fixed top-4 right-4 z-50 px-4 py-3 rounded-md shadow-lg text-sm text-white ${
    isError ? "bg-red-600" : "bg-emerald-600"
  }`;
  toast.classList.remove("hidden");
  setTimeout(() => toast.classList.add("hidden"), 2500);
}

function setView(view) {
  [loginView, dashboardView, courseDetailView].forEach((v) => v.classList.add("hidden"));
  view.classList.remove("hidden");
}

async function apiFetch(path, options = {}) {
  const headers = { "Content-Type": "application/json", ...(options.headers || {}) };
  if (authToken) headers["Authorization"] = `Bearer ${authToken}`;

  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });
  const data = await response.json().catch(() => null);

  
  if (response.status === 401 && path !== "/auth/login") {
    logout();
    throw new Error("Session expired. Please sign in again.");
  }

  if (!response.ok) {
    throw new Error(data?.message || "Something went wrong. Please try again.");
  }

  return data;
}

function statusBadgeClasses(status) {
  switch (status) {
    case "Completed":
      return "bg-emerald-100 text-emerald-700";
    case "In Progress":
      return "bg-amber-100 text-amber-700";
    default:
      return "bg-slate-100 text-slate-600";
  }
}

async function login(username, password) {
  loginError.classList.add("hidden");
  try {
    const data = await apiFetch("/auth/login", {
      method: "POST",
      body: JSON.stringify({ username, password }),
    });
    authToken = data.token;
    currentUsername = data.username;
    sessionStorage.setItem("token", authToken);
    sessionStorage.setItem("username", currentUsername);
    enterApp();
  } catch (err) {
    loginError.textContent = err.message || "Invalid username or password.";
    loginError.classList.remove("hidden");
  }
}

function logout() {
  authToken = null;
  currentUsername = null;
  sessionStorage.removeItem("token");
  sessionStorage.removeItem("username");
  userBar.classList.add("hidden");
  setView(loginView);
}

function enterApp() {
  usernameLabel.textContent = currentUsername;
  userBar.classList.remove("hidden");
  userBar.classList.add("flex");
  loadDashboard();
}

// ---------- Dashboard ----------
async function loadDashboard() {
  setView(dashboardView);
  try {
    const courses = await apiFetch("/courses");
    renderDashboard(courses);
  } catch (err) {
    showToast(err.message, true);
  }
}

function renderDashboard(courses) {
  const myCourses = document.getElementById("myCourses");
  const availableCourses = document.getElementById("availableCourses");
  myCourses.innerHTML = "";
  availableCourses.innerHTML = "";

  courses.forEach((course) => {
    const card = document.createElement("div");
    card.className = "bg-white rounded-lg shadow p-4 flex flex-col justify-between";

    if (course.isEnrolled) {
      card.innerHTML = `
        <div>
          <div class="flex items-center justify-between mb-1">
            <h3 class="font-medium text-slate-800">${course.title}</h3>
            <span class="text-xs px-2 py-0.5 rounded-full ${statusBadgeClasses(course.status)}">${course.status}</span>
          </div>
          <p class="text-sm text-slate-500 mb-3">${course.description}</p>
          <p class="text-xs text-slate-400 mb-1">${course.lessonCount} lessons &middot; ${course.progressPercentage}% complete</p>
          <div class="w-full bg-slate-200 rounded-full h-2 mb-3">
            <div class="bg-indigo-600 h-2 rounded-full" style="width:${course.progressPercentage}%"></div>
          </div>
        </div>
        <button class="open-course text-sm bg-indigo-600 text-white py-1.5 rounded-md hover:bg-indigo-700" data-id="${course.id}">
          Continue
        </button>`;
      myCourses.appendChild(card);
    } else {
      card.innerHTML = `
        <div>
          <h3 class="font-medium text-slate-800 mb-1">${course.title}</h3>
          <p class="text-sm text-slate-500 mb-3">${course.description}</p>
          <p class="text-xs text-slate-400 mb-3">${course.lessonCount} lessons</p>
        </div>
        <button class="enroll-course text-sm bg-slate-800 text-white py-1.5 rounded-md hover:bg-slate-700" data-id="${course.id}">
          Enroll
        </button>`;
      availableCourses.appendChild(card);
    }
  });

  document.querySelectorAll(".open-course").forEach((btn) =>
    btn.addEventListener("click", () => openCourse(Number(btn.dataset.id)))
  );
  document.querySelectorAll(".enroll-course").forEach((btn) =>
    btn.addEventListener("click", () => enrollInCourse(Number(btn.dataset.id)))
  );
}

async function enrollInCourse(courseId) {
  try {
    await apiFetch("/courses/enroll", {
      method: "POST",
      body: JSON.stringify({ courseId }),
    });
    showToast("Enrolled successfully!");
    await loadDashboard(); // Refresh without a full page reload.
  } catch (err) {
    showToast(err.message, true);
  }
}

// ---------- Course Detail ----------
async function openCourse(courseId) {
  currentCourseId = courseId;
  try {
    const course = await apiFetch(`/courses/${courseId}`);
    renderCourseDetail(course);
    setView(courseDetailView);
  } catch (err) {
    showToast(err.message, true);
  }
}

function renderCourseDetail(course) {
  document.getElementById("courseTitle").textContent = course.title;
  document.getElementById("courseDescription").textContent = course.description;
  document.getElementById("courseStatus").textContent = course.status;
  document.getElementById("courseStatus").className = `font-medium ${statusBadgeClasses(course.status).split(" ")[1]}`;
  document.getElementById("courseProgressLabel").textContent = `${course.progressPercentage}% complete`;
  document.getElementById("progressBar").style.width = `${course.progressPercentage}%`;

  const list = document.getElementById("lessonsList");
  list.innerHTML = "";

  course.lessons.forEach((lesson) => {
    const item = document.createElement("div");
    item.className = "bg-white rounded-lg shadow p-4 flex items-start justify-between gap-4";
    item.innerHTML = `
      <div>
        <h4 class="font-medium text-slate-800">${lesson.order}. ${lesson.title}</h4>
        <p class="text-sm text-slate-500 mt-1">${lesson.content}</p>
      </div>
      <button class="complete-lesson shrink-0 text-xs font-medium px-3 py-1.5 rounded-md ${
        lesson.isCompleted
          ? "bg-emerald-100 text-emerald-700 cursor-default"
          : "bg-indigo-600 text-white hover:bg-indigo-700"
      }" data-id="${lesson.id}" ${lesson.isCompleted ? "disabled" : ""}>
        ${lesson.isCompleted ? "✓ Completed" : "Mark Complete"}
      </button>`;
    list.appendChild(item);
  });

  document.querySelectorAll(".complete-lesson").forEach((btn) =>
    btn.addEventListener("click", () => completeLesson(Number(btn.dataset.id)))
  );
}

async function completeLesson(lessonId) {
  try {
    await apiFetch("/courses/lessons/complete", {
      method: "POST",
      body: JSON.stringify({ lessonId }),
    });
    showToast("Lesson marked as complete!");
    await openCourse(currentCourseId); 
  } catch (err) {
    showToast(err.message, true);
  }
}

loginForm.addEventListener("submit", (e) => {
  e.preventDefault();
  const username = document.getElementById("username").value.trim();
  const password = document.getElementById("password").value;
  login(username, password);
});

document.getElementById("logoutBtn").addEventListener("click", logout);
document.getElementById("backBtn").addEventListener("click", loadDashboard);

if (authToken && currentUsername) {
  enterApp();
} else {
  setView(loginView);
}