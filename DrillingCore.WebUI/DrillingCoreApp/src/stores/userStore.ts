import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    userId: null as number | null,
    fullName: '',
    role: '',
    roleId: null as number | null,
    isAuthenticated: false
  }),
  actions: {
    setUser(user: { userId: number, fullName: string, role: string, roleId: number }) {
      this.userId = user.userId
      this.fullName = user.fullName
      this.role = user.role
      
      this.isAuthenticated = true
      this.roleId = user.roleId
    },
    clearUser() {
      this.userId = null
      this.fullName = ''
      this.role = ''
      this.isAuthenticated = false
      this.roleId=null
    }
  }
})
