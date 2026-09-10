"use client";
import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from "@/components/ui/button";
import {
  Card,

  CardContent,
  CardDescription,

  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Switch } from '@/components/ui/switch';
import { toast } from 'react-toastify'; 
import Navbar from '@/components/common/Navbar';

// Loading spinner component
const LoadingSpinner = () => (
  <div className="flex justify-center items-center h-64">
    <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
  </div>
);


// Zod schema for user details with preferences
const userDetailsSchema = z.object({
  firstName: z.string().min(2, {
    message: "First name must be at least 2 characters.",
  }),
  lastName: z.string().min(2, {
    message: "Last name must be at least 2 characters.",
  }),
  email: z.string().email({
    message: "Please enter a valid email address.",
  }),
  preferences: z.object({
    emailNotifications: z.boolean(),
    theme: z.enum(['light', 'dark'], {
      error: "Theme is required",
    }),
  }),
});

// password
const passwordformSchema = z.object({
  currentPassword: z.string().min(8, {
    message: "Current password must be at least 8 characters.",
  }),
  newPassword: z.string().min(8, {
    message: "New password must be at least 8 characters.",
  }),
  confirmPassword: z.string().min(8, {
    message: "Confirm password must be at least 8 characters.",
  }),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ["confirmPassword"],
});

type UserDetails = z.infer<typeof userDetailsSchema>;

export default function Preferences() {
  const [user, setUser] = useState<UserDetails | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const form = useForm<UserDetails>({
    resolver: zodResolver(userDetailsSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      preferences: {
        emailNotifications: true,
        theme: "light",
      },
    },
  });

  const passwordForm = useForm<z.infer<typeof passwordformSchema>>({
    resolver: zodResolver(passwordformSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  useEffect(() => {
    loadUserData();
  }, []);

  useEffect(() => {
    if (user) {
      form.reset({
        firstName: user.firstName || "",
        lastName: user.lastName || "",
        email: user.email || "",
        preferences: {
          emailNotifications: user.preferences?.emailNotifications || true,
          theme: user.preferences?.theme || "light",
        },
      });
    }
  }, [user]);

  const loadUserData = async () => {
    try {
      const userString = localStorage.getItem('user');
      if (userString) {
        const userData = JSON.parse(userString);
        setUser(userData);
      } else {
        setError('No user data found. Please log in.');
      }
    } catch (err) {
      setError('Failed to load user data');
      console.error('Error loading user data:', err);
    } finally {
      setLoading(false);
    }
  };
    // change password
 const onSubmitPasswordChange = async (data: z.infer<typeof passwordformSchema>) => {
    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error("Authentication token not found. Please log in.");
      }

      const response = await fetch(
        'https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/Profile/password',
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
          body: JSON.stringify({
            currentPassword: data.currentPassword,
            newPassword: data.newPassword,
          }),
        }
      );

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      console.log("Password changed successfully:", result);

      // Show success notification
      toast.success("Your password has been updated successfully!");
      // clear the form 
      passwordForm.reset;
      // redirect
      navigate('/dashboard', { replace: true })

    } catch (error) {
      console.error("Error changing password:", error);
      toast.error("Failed to change your password. Please try again.");
    }
  };


  const onSubmit = async (data: UserDetails) => {
    try {
       const token = localStorage.getItem('authToken');

      if (!token) {
        throw new Error("Authentication token not found. Please log in.");
      }
      const response = await fetch(`https://hackathonteam6api-gbabgfcsg2cngygr.canadacentral-01.azurewebsites.net/api/v1/Profile`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      console.log("Update successful:", result);

      // Update local storage with new user data
      localStorage.setItem('user', JSON.stringify(data));
      setUser(data);

      // Show success notification
      toast.success("Your profile has been updated successfully!");
      // clear the form
      form.reset;
      // redirect home
      navigate('/dashboard', { replace: true })

      
    } catch (error) {
      console.error("Error updating profile:", error);
      toast.error("Failed to update your profile. Please try again.");
    }
  };

  // Show loading spinner while fetching data
  if (loading) {
    return (
      <div className='w-full flex-col gap-4 p-6'>
        <h1 className="text-2xl font-bold mb-6">Preferences</h1>
        <LoadingSpinner />
      </div>
    );
  }

  // Show error message if there's an error
  if (error) {
    return (
      <div className='w-full flex-col gap-4 p-6'>
        <h1 className="text-2xl font-bold mb-6">Preferences</h1>
        <div className="text-center py-12">
          <p className="text-red-500 mb-4">{error}</p>
          <Button onClick={() => navigate('/')}>Go to Login</Button>
        </div>
      </div>
    );
  }

  return (
    <div className='w-full flex flex-col gap-6 p-6'>
       <Navbar/>
      <h1 className="text-2xl font-bold">Preferences</h1>
     

      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle>Update Your Profile</CardTitle>
          <CardDescription>
            Edit your personal information, notification settings, and theme.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <FormField
                  control={form.control}
                  name="firstName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>First Name</FormLabel>
                      <FormControl>
                        <Input placeholder="First name" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="lastName"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Last Name</FormLabel>
                      <FormControl>
                        <Input placeholder="Last name" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Email</FormLabel>
                    <FormControl>
                      <Input placeholder="Email address" type="email" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Email Notifications */}
              <FormField
                control={form.control}
                name="preferences.emailNotifications"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Email Notifications</FormLabel>
                      <FormDescription>
                        Receive emails about your account activity.
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />

              {/* Theme Selection */}
              <FormField
                control={form.control}
                name="preferences.theme"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Theme</FormLabel>
                    <FormControl>
                      <select
                        className="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                        {...field}
                      >
                        <option value="light">Light</option>
                        <option value="dark">Dark</option>
                      </select>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <Button type="submit">Save Changes</Button>
            </form>
          </Form>
        </CardContent>
      </Card>


      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle>Change Password</CardTitle>
          <CardDescription>Update your account password</CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...passwordForm}>
            <form onSubmit={passwordForm.handleSubmit(onSubmitPasswordChange)} className="space-y-6">
              <FormField
                control={passwordForm.control}
                name="currentPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Current Password</FormLabel>
                    <FormControl>
                      <Input placeholder="Current password" type="password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={passwordForm.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>New Password</FormLabel>
                    <FormControl>
                      <Input placeholder="New password" type="password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={passwordForm.control}
                name="confirmPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Confirm New Password</FormLabel>
                    <FormControl>
                      <Input placeholder="Confirm new password" type="password" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              
              
              <Button type="submit">Change Password</Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}