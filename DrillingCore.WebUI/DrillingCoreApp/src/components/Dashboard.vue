<template>
  <div class="dashboard-container" :class="{ 'dark-theme': isDarkTheme }">
    <!-- Sidebar -->
    <aside class="sidebar">
      <div class="sidebar-header">
        <h2>DrillingCore</h2>
      </div>
      <nav>
        <ul class="menu">
          <li 
            v-for="item in menuItems" 
            :key="item.name"
            :class="['menu-item', { active: currentSection === item.name }]"
            @click.prevent="setSection(item.name)"
          >
            <i :class="item.icon"></i>
            <span>{{ item.label }}</span>
          </li>
        </ul>
      </nav>
      <div class="theme-toggle">
        <label class="switch">
          <input type="checkbox" v-model="isDarkTheme">
          <span class="slider round"></span>
        </label>
        <span>Dark Theme</span>
      </div>
    </aside>

    <!-- Main Content -->
    <main class="main-content">
      <header class="main-header">
        <div class="header-info">
          <h1>{{ currentSection }}</h1>
        </div>
        <div class="header-actions">
          <button class="btn logout-btn" @click="logout">Logout</button>
        </div>
      </header>
      
      <section class="content-area container fade-in">
        <!-- Если активная вкладка - Projects, то рендерим ProjectsSection с ref -->
        <ProjectsSection 
          v-if="currentSection === 'Projects'" 
          ref="projectsSectionRef"
          @open-project-modal="handleOpenProjectModal" 
          @open-participant-modal="handleOpenParticipantModal" 
        />
        <!-- Для других секций используем динамический компонент -->
        <component 
          v-else
          :is="activeSectionComponent" 
          @open-project-modal="handleOpenProjectModal" 
          @open-participant-modal="handleOpenParticipantModal" 
        />
      </section>
    </main>

    <!-- Модальные окна -->
    <ProjectModal 
      v-if="showProjectModal" 
      :projectId="currentProjectId ?? 0" 
      @close="closeProjectModal" 
      @project-saved="handleProjectSaved"
    />
    <ParticipantModal 
      v-if="showParticipantModal" 
      :projectId="currentProjectId ?? 0" 
      @close="closeParticipantModal" 
      @participants-updated="handleParticipantsUpdated"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import ProjectsSection from './ProjectsSection.vue';
import EquipmentSection from './EquipmentSection.vue';
import UsersSection from './UsersSection.vue';
import FormsSection from './FormsSection.vue';
import ReportsSection from './ReportsSection.vue';
import ProjectModal from './ProjectModal.vue';
import ParticipantModal from './ParticipantModal.vue';

export default defineComponent({
  name: 'Dashboard',
  components: {
    ProjectsSection,
    EquipmentSection,
    UsersSection,
    FormsSection,
    ReportsSection,
    ProjectModal,
    ParticipantModal
  },
  setup() {
    const router = useRouter();
    const currentSection = ref('Projects');
    const isDarkTheme = ref(false);
    const showProjectModal = ref(false);
    const showParticipantModal = ref(false);
    const currentProjectId = ref<number | null>(null);
    const projectsSectionRef = ref(null);

    const menuItems = [
      { name: 'Projects', label: 'Projects', icon: 'bi bi-folder-fill' },
      { name: 'Equipment', label: 'Equipment', icon: 'bi bi-truck' },
      { name: 'Users', label: 'Users', icon: 'bi bi-people-fill' },
      { name: 'Forms', label: 'Forms', icon: 'bi bi-file-earmark-text' },
      { name: 'Reports', label: 'Reports', icon: 'bi bi-bar-chart-line-fill' }
    ];

    const activeSectionComponent = computed(() => {
      switch (currentSection.value) {
        case 'Projects': return ProjectsSection;
        case 'Equipment': return EquipmentSection;
        case 'Users': return UsersSection;
        case 'Forms': return FormsSection;
        case 'Reports': return ReportsSection;
        default: return ProjectsSection;
      }
    });

    function setSection(section: string) {
      currentSection.value = section;
      console.log("Active section set to:", currentSection.value);
    }

    function handleOpenProjectModal(projectId?: number) {
      console.log("Dashboard: Opening project modal for projectId:", projectId);
      currentProjectId.value = projectId ?? null;
      showProjectModal.value = true;
    }

    function closeProjectModal() {
      console.log("Dashboard: Closing project modal");
      showProjectModal.value = false;
    }

    function handleProjectSaved() {
      console.log("Dashboard: Project saved. Updating project list.");
      // Если текущая вкладка - Projects, вызываем loadProjects через ref
      if (currentSection.value === 'Projects' && projectsSectionRef.value?.loadProjects) {
        projectsSectionRef.value.loadProjects();
      }
      closeProjectModal();
    }

    function handleOpenParticipantModal(projectId?: number) {
      console.log("Dashboard: Opening participant modal for projectId:", projectId);
      currentProjectId.value = projectId ?? null;
      showParticipantModal.value = true;
    }

    function closeParticipantModal() {
      console.log("Dashboard: Closing participant modal");
      showParticipantModal.value = false;
    }

    function handleParticipantsUpdated() {
      console.log("Dashboard: Participants updated.");
      closeParticipantModal();
    }

    function logout() {
      localStorage.removeItem('auth_token');
      router.push('/');
    }

    return {
      currentSection,
      isDarkTheme,
      menuItems,
      activeSectionComponent,
      setSection,
      showProjectModal,
      currentProjectId,
      handleOpenProjectModal,
      closeProjectModal,
      handleProjectSaved,
      showParticipantModal,
      handleOpenParticipantModal,
      closeParticipantModal,
      handleParticipantsUpdated,
      logout,
      router,
      projectsSectionRef
    };
  }
});
</script>

<style scoped>
.dashboard-container {
  display: flex;
  min-height: 100vh;
  background-color: var(--bg-color, #ffffff);
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

/* Светлая тема */
:root {
  --bg-color: #ffffff;
  --sidebar-bg: #f0f0f0;
  --header-bg: #ffffff;
  --content-bg: #fafafa;
  --primary-color: #1976d2;
  --primary-dark: #115293;
  --general-border-color-light: #ccc;
  --active-border-color: #ff80ab;
  --active-text-color: #ff80ab;
  --active-bg-color: #e0e0e0;
  --menu-text-light: #333333;
  --logout-text-light: #000;
}

/* Тёмная тема */
.dark-theme {
  --bg-color: #121212;
  --sidebar-bg: #1e1e1e;
  --header-bg: #1f1f1f;
  --content-bg: #282828;
  --primary-color: #bb86fc;
  --primary-dark: #985eff;
  --general-border-color-dark: #333;
  --active-border-color: #ff80ab;
  --active-text-color: #ff80ab;
  --active-bg-color: #333;
  --menu-text-dark: #e0e0e0;
  --logout-text-dark: #fff;
}

.dashboard-container {
  background-color: var(--bg-color);
}

/* Sidebar */
.sidebar {
  width: 260px;
  background-color: var(--sidebar-bg);
  border-right: 1px solid var(--general-border-color-light);
  padding: 1.5rem 1rem;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 2px 0 8px rgba(0, 0, 0, 0.1);
}

.dark-theme .sidebar {
  border-right: 1px solid var(--general-border-color-dark);
}

.sidebar-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.sidebar-header h2 {
  font-size: 1.8rem;
  color: var(--primary-dark);
  margin: 0;
}

.menu {
  list-style: none;
  padding: 0;
  margin: 0;
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 0.75rem 1rem;
  margin-bottom: 0.5rem;
  border: 1px solid transparent;
  border-radius: 6px;
  cursor: pointer;
  transition: background-color 0.3s ease, border 0.3s ease, transform 0.3s ease, color 0.3s ease;
  color: var(--menu-text-light);
}

.dark-theme .menu-item {
  color: var(--menu-text-dark);
}

.menu-item i {
  font-size: 1.2rem;
  margin-right: 0.5rem;
  transition: transform 0.3s ease;
}

.menu-item:hover {
  background-color: #e0e0e0;
  transform: scale(1.02);
}

.menu-item.active {
  border: 2px solid var(--active-border-color) !important;
  background-color: var(--active-bg-color);
  color: var(--active-text-color) !important;
}

.dark-theme .menu-item:hover {
  background-color: #333;
  transform: scale(1.02);
}

.dark-theme .menu-item.active:hover {
  background-color: inherit;
  transform: none;
}

.theme-toggle {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-top: 1rem;
}

.switch {
  position: relative;
  display: inline-block;
  width: 40px;
  height: 20px;
  margin-right: 0.5rem;
}

.switch input {
  opacity: 0;
  width: 0;
  height: 0;
}

.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  transition: 0.4s;
  border-radius: 20px;
}

.slider:before {
  position: absolute;
  content: "";
  height: 16px;
  width: 16px;
  left: 2px;
  bottom: 2px;
  background-color: white;
  transition: 0.4s;
  border-radius: 50%;
}

.switch input:checked + .slider {
  background-color: var(--primary-color);
}

.switch input:checked + .slider:before {
  transform: translateX(20px);
}

.theme-toggle span {
  color: var(--menu-text-light);
  font-size: 0.9rem;
}

/* Main Content */
.main-content {
  flex-grow: 1;
  display: flex;
  flex-direction: column;
}

.main-header {
  background-color: var(--header-bg);
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid var(--general-border-color-light);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.dark-theme .main-header {
  border-bottom: 1px solid var(--general-border-color-dark);
}

.header-info h1 {
  margin: 0;
  font-size: 1.75rem;
  color: var(--menu-text-light);
}

.dark-theme .header-info h1 {
  color: var(--menu-text-dark);
}

.logout-btn {
  background-color: var(--primary-color);
  border: none;
  color: var(--logout-text-light);
  padding: 0.5rem 1rem;
  border-radius: 4px;
  transition: background 0.3s ease, transform 0.3s ease;
}

.dark-theme .logout-btn {
  color: var(--logout-text-dark);
}

.logout-btn:hover {
  background-color: var(--primary-dark);
  transform: scale(1.05);
}

/* Content Area */
.content-area {
  flex: 1;
  padding: 2rem;
  background-color: var(--content-bg);
  animation: fadeIn 0.5s ease-in-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Responsive */
@media (max-width: 768px) {
  .dashboard-container {
    flex-direction: column;
  }
  .sidebar {
    width: 100%;
    padding: 0.75rem 1rem;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;
  }
  .sidebar-header h2 {
    font-size: 1.5rem;
  }
  .menu {
    display: flex;
    flex-wrap: wrap;
  }
  .menu-item {
    margin: 0 0.25rem;
    padding: 0.5rem;
    font-size: 0.9rem;
  }
  .main-header {
    padding: 0.75rem 1rem;
  }
  .content-area {
    padding: 1rem;
  }
}
</style>
